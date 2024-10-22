using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.AuditEntities;
using Ecommerce.Core.Entities;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Infrastructure
{
    public class AuditDbContext : EcommerceAuditContext
    {

        public AuditDbContext( )
        {


        }
        public AuditDbContext( DbConnection connection )
        {

            //  this.Database.SetDbConnection(connection);
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {


            modelBuilder.Entity<AuditLog>().Ignore(s => s.IsDeleted);
            modelBuilder.Entity<AuditLog>().Ignore(s => s.IsActive);
            base.OnModelCreating(modelBuilder);

        }
    }
}
