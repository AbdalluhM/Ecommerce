using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class TaxEventsAudit:EntityBase
    {
        public long EventId { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string JsonData { get; set; }
        public string EventType { get; set; }
        public string User { get; set; }
        public string AuditAction { get; set; }
        public DateTime? AuditDate { get; set; }
        public string UserName { get; set; }
    }
}
