using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Logs;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static Ecommerce.Core.Consts.Logs.LogsConst;

namespace Ecommerce.Core.Infrastructure
{
    public class AuditLogInterceptor : ISaveChangesInterceptor
    {


        public InterceptionResult<int> SavingChanges( DbContextEventData eventData, InterceptionResult<int> result )
        {
            ScanAndLog(eventData);
            return result;

        }





        public int SavedChanges( SaveChangesCompletedEventData eventData, int result )
        {
            ScanAndLog(eventData);
            return result;
        }

        public void SaveChangesFailed( DbContextErrorEventData eventData )
        {
        }

        public async ValueTask<int> SavedChangesAsync( SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default )
        {
            return result;
        }

        public Task SaveChangesFailedAsync( DbContextErrorEventData eventData, CancellationToken cancellationToken = default )
        {
            return null;
        }

        public async ValueTask<InterceptionResult<int>> SavingChangesAsync( DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default )
        {
            await ScanAndLogAsync(eventData, cancellationToken);

            return result;
        }
        public void ScanAndLog( DbContextEventData eventData, CancellationToken cancellationToken = default )
        {
            Task.Run(( ) => ScanAndLogAsync(eventData, cancellationToken)).GetAwaiter().GetResult();
        }
        public async Task ScanAndLogAsync( DbContextEventData eventData, CancellationToken cancellationToken = default )
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            eventData.Context.ChangeTracker.DetectChanges();
            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (LogTableNames.Contains(entry.Entity.GetType().Name) || LogTableNames.Select(x => x + EntitySuffix).Contains(entry.Entity.GetType().Name))
                {
                    var auditEntity = entry.State switch
                    {
                        EntityState.Deleted => await CreateAuditAsync(eventData, entry, (int)AuditActionTypesEnum.Delete),
                        EntityState.Modified => await CreateAuditAsync(eventData, entry, (int)AuditActionTypesEnum.Update),
                        EntityState.Added => await CreateAuditAsync(eventData, entry, (int)AuditActionTypesEnum.Create),
                        _ => null
                    };
                }

            }

        }
        private async Task<AuditLog> CreateAuditAsync( DbContextEventData eventData, EntityEntry entry, int auditActionTypeId, bool commit = true )
        {
            AuditLog auditLogEntity = null;//= new AuditLog();


            //Add Master Audit Log 

            auditLogEntity = new AuditLog
            {
                AuditActionTypeId = auditActionTypeId,
                CreateDate = DateTime.UtcNow,
                IsActive = true,
                TableName = entry.Metadata.DisplayName(),
                TempPrimaryKey
                = auditActionTypeId == (int)AuditActionTypesEnum.Create
                ? entry.Properties.FirstOrDefault(x => x.Metadata.Name == EntityTempGUIDColumenName)?.CurrentValue?.ToString()
                : entry.Properties.FirstOrDefault(x => x.Metadata.Name == EntityIdColumnName || x.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(),

                PrimaryKey
                = auditActionTypeId == (int)AuditActionTypesEnum.Create
                ? null
                : int.TryParse(entry.Properties.FirstOrDefault(x => x.Metadata.Name == EntityIdColumnName || x.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(), out int Id) ? Id : null,


            };
            if (auditLogEntity != null)
                foreach (var prop in entry.Properties.ToList())
                {

                    auditLogEntity.AuditLogDetails.Add(new AuditLogDetail
                    {
                        //JsonConvert.SerializeObject(entry.Properties.Select(x => "[" + x.Metadata.Name + ":" + x.CurrentValue?.ToString() + "],").ToList())
                        NewValue = prop.CurrentValue?.ToString(),
                        OldValue
                        = auditActionTypeId == (int)AuditActionTypesEnum.Create
                        ? ""
                        : prop.OriginalValue?.ToString(),
                        FieldName = prop.Metadata.Name,

                    });

                }

            if (commit)
            {
                await CommitAuditAsync(eventData, auditLogEntity);

            }
            return auditLogEntity;
        }

        private AuditLog CreateAudit( DbContextEventData eventData, EntityEntry entry, int auditActionTypeId, bool commit = true )
        {
            return Task.Run(( ) => CreateAuditAsync(eventData, entry, auditActionTypeId, commit)).GetAwaiter().GetResult();

        }
        private void CommitAudit( DbContextEventData eventData, AuditLog entity )
        {
            Task.Run(( ) => CommitAuditAsync(eventData, entity));
        }
        private async Task CommitAuditAsync( DbContextEventData eventData, AuditLog entity )
        {
            try
            {
                if (entity != null)
                {
                    using var newContext = new AuditDbContext();
                    newContext.Database.SetDbConnection(eventData.Context.Database.GetDbConnection());
                    await newContext.AuditLogs.AddAsync(entity);
                    await newContext.SaveChangesAsync();
                }

            }
            catch
            {
                throw;
            }
        }

        #region Old Commented
        //string CreateAddedMessage(EntityEntry entry)
        //    => entry.Properties.Aggregate(
        //        $"Inserting {entry.Metadata.DisplayName()} with ",
        //        (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");

        //string CreateModifiedMessage(EntityEntry entry)
        //    => entry.Properties.Where(property => property.IsModified || property.Metadata.IsPrimaryKey()).Aggregate(
        //        $"Updating {entry.Metadata.DisplayName()} with ",
        //        (auditString, property) => auditString + $"{property.Metadata.Name}:'Old value : {property.OriginalValue} -'New Value {property.CurrentValue}\n' ");

        //string CreateDeletedMessage(EntityEntry entry)
        //    => entry.Properties.Where(property => property.Metadata.IsPrimaryKey()).Aggregate(
        //        $"Deleting {entry.Metadata.DisplayName()} with ",
        //        (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");

        //return result;
        //return base.SavingChanges(eventData, result);   
        //{
        //    ApplicationDbContext context = (ApplicationDbContext)eventData.Context;

        //    //foreach (var prop in entry.Properties)
        //    //{
        //    //    if (prop.CurrentValue?.ToString() != prop.OriginalValue?.ToString())
        //    //    {
        //    var prop = entry.Properties.FirstOrDefault(s => s.CurrentValue?.ToString() == "Name");

        //    AuditLog auditLogEntity = new AuditLog
        //    {
        //        NewValue = prop.CurrentValue.ToString(),
        //        OldValue = "",
        //        TableName = entry.Metadata.DisplayName(),
        //        Type = "Create",
        //        PrimayKey = string.Empty,

        //    };
        //    context.AuditLogs.Add(auditLogEntity);


        //    //    }
        //    //}

        //private string CreateAudit( DbContextEventData eventData, EntityEntry entry )
        //{
        //    ApplicationDbContext context = (ApplicationDbContext)eventData.Context;

        //    //foreach (var prop in entry.Properties)
        //    //{
        //    //    if (prop.CurrentValue?.ToString() != prop.OriginalValue?.ToString())
        //    //    {
        //    var prop = entry.Properties.FirstOrDefault(s => s.CurrentValue?.ToString() == "Name");

        //    AuditLog auditLogEntity = new AuditLog
        //    {
        //        NewValue = prop.CurrentValue.ToString(),
        //        OldValue = "",
        //        TableName = entry.Metadata.DisplayName(),
        //        Type = "Create",
        //        PrimayKey = string.Empty,

        //    };
        //    context.AuditLogs.Add(auditLogEntity);


        //    //    }
        //    //}


        //    return entry.Properties.Aggregate(
        //             $"Inserting {entry.Metadata.DisplayName()} with ",
        //             ( auditString, property ) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");
        //}
        #endregion
    }

}
