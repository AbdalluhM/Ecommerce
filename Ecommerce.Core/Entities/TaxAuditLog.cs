using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class TaxAuditLog:EntityBase
    {
        public Guid Id { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string AuditType { get; set; }
        public string CreateBy { get; set; }
        public string TableName { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string ChangedColumns { get; set; }
    }
}
