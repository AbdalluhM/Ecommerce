using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AuditLog:EntityBase
    {
        public AuditLog()
        {
            AuditLogDetails = new HashSet<AuditLogDetail>();
        }

        public int Id { get; set; }
        public string TableName { get; set; }
        public int AuditActionTypeId { get; set; }
        public int? PrimaryKey { get; set; }
        public DateTime CreateDate { get; set; }
        public string TempPrimaryKey { get; set; }

        public virtual AuditActionType AuditActionType { get; set; }
        public virtual ICollection<AuditLogDetail> AuditLogDetails { get; set; }
    }
}
