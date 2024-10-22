using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AuditLogDetail:EntityBase
    {
        public int Id { get; set; }
        public int AuditLogId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public virtual AuditLog AuditLog { get; set; }
    }
}
