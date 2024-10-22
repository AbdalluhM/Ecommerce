using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AuditActionType:EntityBase
    {
        public AuditActionType()
        {
            AuditLogs = new HashSet<AuditLog>();
        }

        public int Id { get; set; }
        public string ActionType { get; set; }

        public virtual ICollection<AuditLog> AuditLogs { get; set; }
    }
}
