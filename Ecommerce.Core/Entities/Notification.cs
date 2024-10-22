using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Notification:EntityBase
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? NotificationActionId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ReadAt { get; set; }
        public int? ReadByEmployeeId { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        public bool IsRead { get; set; }
        public bool IsHide { get; set; }
        public string TicketRefrences { get; set; }
        public DateTime? HiddenAt { get; set; }
        public int? HiddenByEmployeeId { get; set; }
        public int? InvoiceId { get; set; }
        public int? LicenceId { get; set; }
        public string TicketId { get; set; }
        public int? RefundRequestId { get; set; }

        public virtual Employee CreatedByEmployee { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee HiddenByEmployee { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual License Licence { get; set; }
        public virtual NotificationAction NotificationAction { get; set; }
        public virtual Employee ReadByEmployee { get; set; }
        public virtual RefundRequest RefundRequest { get; set; }
    }
}
