using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class RefundRequestStatu:EntityBase
    {
        public RefundRequestStatu()
        {
            RefundRequests = new HashSet<RefundRequest>();
        }

        public int Id { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<RefundRequest> RefundRequests { get; set; }
    }
}
