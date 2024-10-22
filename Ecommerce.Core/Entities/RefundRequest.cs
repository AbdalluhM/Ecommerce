using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class RefundRequest:EntityBase
    {
        public RefundRequest()
        {
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int RefundRequestStatusId { get; set; }
        public string Reason { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual RefundRequestStatu RefundRequestStatus { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
