using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.Entities;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Ecommerce.Core.Helpers.Extensions;
using System;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading.Tasks;
using Action = Ecommerce.Core.Entities.Action;
using Microsoft.Extensions.Logging;
using Connection = Ecommerce.Core.Entities.Connection;

namespace Ecommerce.Core.Infrastructure
{
    //public static readonly ILoggerFactory consoleLoggerFactory
    //        = new LoggerFactory(new [] {
    //              new ConsoleLoggerProvider((category, level) =>
    //                category == DbLoggerCategory.Database.Command.Name &&
    //                level == LogLevel.Information, true)
    //            });
    public class ApplicationDbContext : EcommerceContext
    { 
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbConnection connection)
        {
           // this.Database.SetDbConnection(connection);
        }




        //public ApplicationDbContext( DbContextOptions<EcommerceContext> options ) : base(options)
        //{

        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

         #if DEBUG
            optionsBuilder.LogTo(Console.WriteLine)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
          #endif
            optionsBuilder.AddInterceptors(new AuditLogInterceptor());
            //optionsBuilder.UseLoggerFactory()
                base.OnConfiguring(optionsBuilder);
            }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var jsonvalueMethodInfo = typeof(Json).GetRuntimeMethod(nameof(Json.Value), new[] { typeof(string), typeof(string) });

            modelBuilder.HasDbFunction(jsonvalueMethodInfo).HasTranslation(args => SqlFunctionExpression.Create("JSON_VALUE", args, typeof(string), null)); 

            #region Schema
            modelBuilder.Entity<Country>().ToTable("Country", "LookUp");
            modelBuilder.Entity<Currency>().ToTable("Currency", "LookUp");
            modelBuilder.Entity<EmployeeType>().ToTable("EmployeeType", "LookUp");
            modelBuilder.Entity<SubscriptionType>().ToTable("SubscriptionType", "LookUp");



            modelBuilder.Entity<CountryCurrency>().ToTable("CountryCurrency", "Admin");
            modelBuilder.Entity<CountryCurrency>().ToTable("AddOnPrice", "Admin");

            modelBuilder.Entity<EmployeeCountry>().ToTable("EmployeeCountry", "Admin").HasOne(o => o.Employee).WithMany(o => o.EmployeeCountries).HasForeignKey(c => c.EmployeeId);

            modelBuilder.Entity<EmployeeCountry>().ToTable("EmployeeCountry", "Admin").HasOne(o => o.CountryCurrency).WithMany(o => o.EmployeeCountries).HasForeignKey(c => c.CountryCurrencyId);
            //modelBuilder.Entity<VersionRelease>().ToTable("VersionRelease", "Admin");

            #endregion

            #region Log Tables
            //modelBuilder.Entity<Customer>().Property(x => x.TempGuid).HasDefaultValueSql("newsequentialid()");
            //modelBuilder.Entity<Tax>().Property(x => x.TempGuid).HasDefaultValueSql("newsequentialid()");
            //modelBuilder.Entity<Customer>().Property(x => x.TempGuid).HasDefaultValue( Guid.NewGuid().ToString());
            //modelBuilder.Entity<Tax>().Property(x => x.TempGuid).HasDefaultValue(Guid.NewGuid().ToString());

            #endregion

            #region Ignore Dynamic Variables for not existing fields
            modelBuilder.Entity<Country>().Ignore(s => s.IsDeleted);
            //modelBuilder.Entity<Country>().Ignore(s => s.IsActive);
            modelBuilder.Entity<Currency>().Ignore(s => s.IsDeleted);
            //modelBuilder.Entity<Currency>().Ignore(s => s.IsActive);

            modelBuilder.Entity<EmployeeType>().Ignore(s => s.IsDeleted);

            modelBuilder.Entity<EmployeeCountry>().Ignore(s => s.IsActive);
            modelBuilder.Entity<EmployeeCountry>().Ignore(s => s.IsDeleted);

            modelBuilder.Entity<FileType>().Ignore(f => f.IsActive);
            modelBuilder.Entity<FileType>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<FileStorage>().Ignore(f => f.IsActive);
            modelBuilder.Entity<FileStorage>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<AddOnTag>().Ignore(f => f.IsActive);
            modelBuilder.Entity<AddOnTag>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<ViewMissingAddOnPrice>().Ignore(f => f.IsActive);
            modelBuilder.Entity<ViewMissingAddOnPrice>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<ViewDashboardTotal>().Ignore(f => f.IsActive);
            modelBuilder.Entity<ViewDashboardTotal>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<AddOnLabel>().Ignore(f => f.IsActive);

            modelBuilder.Entity<ModuleTag>().Ignore(f => f.IsActive);
            modelBuilder.Entity<ModuleTag>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<SubscriptionType>().Ignore(f => f.IsActive);
            modelBuilder.Entity<SubscriptionType>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<ApplicationTag>().Ignore(f => f.IsActive);
            modelBuilder.Entity<ApplicationTag>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<ApplicationLabel>().Ignore(f => f.IsActive);

            modelBuilder.Entity<ViewMissingVersionPrice>().Ignore(f => f.IsActive);
            modelBuilder.Entity<ViewMissingVersionPrice>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<VersionRelease>().Ignore(f => f.IsActive);
            modelBuilder.Entity<VersionRelease>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<ContactUsHelpOption>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<Industry>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<Industry>().Ignore(f => f.IsActive);

            modelBuilder.Entity<CompanySize>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<CompanySize>().Ignore(f => f.IsActive);

            modelBuilder.Entity<DexefBranch>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<WishListApplication>().Ignore(f => f.IsActive);
            modelBuilder.Entity<WishListApplication>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<WishListAddOn>().Ignore(f => f.IsActive);
            modelBuilder.Entity<WishListAddOn>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<Customer>().Ignore(f => f.IsActive);
            modelBuilder.Entity<Customer>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerReview>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerReview>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerReviewStatu>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerReviewStatu>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerMobile>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerMobile>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerEmail>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerEmail>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerMobile>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerMobile>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerStatu>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerStatu>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerSmsverification>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerSmsverification>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerEmailVerification>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerEmailVerification>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<DownloadVersionLog>().Ignore(f => f.IsActive);
            modelBuilder.Entity<DownloadVersionLog>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<RefreshToken>().Ignore(f => f.IsActive);
            modelBuilder.Entity<RefreshToken>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CountryPaymentMethod>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CountryPaymentMethod>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<VersionSubscription>().Ignore(f => f.IsActive);
            modelBuilder.Entity<VersionSubscription>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<AddonSubscription>().Ignore(f => f.IsActive);
            modelBuilder.Entity<AddonSubscription>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerSubscription>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerSubscription>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<License>().Ignore(f => f.IsActive);
            modelBuilder.Entity<License>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<Invoice>().Ignore(f => f.IsActive);
            modelBuilder.Entity<Invoice>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<InvoiceDetail>().Ignore(f => f.IsActive);
            modelBuilder.Entity<InvoiceDetail>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<InvoiceStatu>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<InvoiceType>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<LicenseStatu>().Ignore(f => f.IsActive);
            modelBuilder.Entity<LicenseStatu>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<RequestActivationKey>().Ignore(f => f.IsActive);
            modelBuilder.Entity<RequestActivationKey>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<RequestChangeDevice>().Ignore(f => f.IsActive);
            modelBuilder.Entity<RequestChangeDevice>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<RefundRequest>().Ignore(f => f.IsActive);
            modelBuilder.Entity<RefundRequest>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<ReasonChangeDevice>().Ignore(f => f.IsActive);
            modelBuilder.Entity<ReasonChangeDevice>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<PaymentType>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<Contract>().Ignore(f => f.IsActive);
            modelBuilder.Entity<Contract>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<RequestChangeDeviceStatu>().Ignore(f => f.IsActive);
            modelBuilder.Entity<RequestChangeDeviceStatu>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<LicenseFile>().Ignore(f => f.IsActive);
            modelBuilder.Entity<LicenseFile>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<LicenseLog>().Ignore(f => f.IsActive);
            modelBuilder.Entity<LicenseLog>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<LicenseActionType>().Ignore(f => f.IsActive);
            modelBuilder.Entity<LicenseActionType>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<ViewLicenseRequest>().Ignore(f => f.IsActive);
            modelBuilder.Entity<ViewLicenseRequest>().Ignore(f => f.IsDeleted);

            //modelBuilder.Entity<Role>().Ignore(f => f.IsActive);
            //modelBuilder.Entity<Role>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<RolePageAction>().Ignore(f => f.IsActive);
            modelBuilder.Entity<RolePageAction>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<PageAction>().Ignore(f => f.IsActive);
            modelBuilder.Entity<PageAction>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<Page>().Ignore(f => f.IsActive);


            modelBuilder.Entity<Action>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<Notification>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<Notification>().Ignore(f => f.IsActive);
            modelBuilder.Entity<NotificationAction>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<NotificationAction>().Ignore(f => f.IsActive);
            modelBuilder.Entity<NotificationActionType>().Ignore(f => f.IsDeleted);
            // modelBuilder.Entity<NotificationStatu>().Ignore(f => f.IsDeleted);
            //modelBuilder.Entity<NotificationUserType>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<CustomerCard>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CustomerCard>().Ignore(f => f.IsDeleted);



            modelBuilder.Entity<AuditActionType>().Ignore(f => f.IsActive);
            modelBuilder.Entity<AuditActionType>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<AuditLog>().Ignore(f => f.IsActive);
            modelBuilder.Entity<AuditLog>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<AuditLogDetail>().Ignore(f => f.IsActive);
            modelBuilder.Entity<AuditLogDetail>().Ignore(f => f.IsDeleted);


            modelBuilder.Entity<CurrencyTable>().Ignore(f => f.IsActive);
            modelBuilder.Entity<CurrencyTable>().Ignore(f => f.IsDeleted);

            modelBuilder.Entity<DevicesType>().Ignore(f => f.IsActive);
            modelBuilder.Entity<DevicesType>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<TableName>().Ignore(f => f.IsActive);
            modelBuilder.Entity<TableName>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<Ticket>().Ignore(f => f.IsActive);
            modelBuilder.Entity<Ticket>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<TicketStatu>().Ignore(f => f.IsActive);
            modelBuilder.Entity<TicketStatu>().Ignore(f => f.IsDeleted); 
            modelBuilder.Entity<TicketType>().Ignore(f => f.IsActive);
            modelBuilder.Entity<TicketType>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<ChatMessage>().Ignore(f => f.IsActive);
            modelBuilder.Entity<Connection>().Ignore(f => f.IsActive);
            modelBuilder.Entity<Connection>().Ignore(f => f.IsDeleted);
            modelBuilder.Entity<ChatMessage>().Ignore(f => f.IsDeleted);


            modelBuilder.Entity<SimpleDatabas>().Ignore(f => f.IsActive);
            modelBuilder.Entity<SimpleDatabas>().Ignore(f => f.IsDeleted);
            #region Apply Global Filters

            //if (!Database.GetDbConnection().GetType().Name.StartsWith("Effort", StringComparison.Ordinal))
            //{
            //TODO:UnComment this
            modelBuilder.ApplyGlobalFilters<IEntityBase>(e => e.IsActive == true, "IsActive");
            modelBuilder.ApplyGlobalFilters<IEntityBase>(e => e.IsDeleted == false, "IsDeleted");
            //}

            #endregion
            #endregion
           
            base.OnModelCreating(modelBuilder);

        }

        public virtual void Commit()
        {
            SaveChanges();
        }

        public virtual async Task<int> CommitAsync()
        {
            return await SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }

}

