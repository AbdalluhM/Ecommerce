using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CustomerEmail:EntityBase
    {
        public CustomerEmail()
        {
            CustomerEmailVerifications = new HashSet<CustomerEmailVerification>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public int SendCount { get; set; }
        public DateTime? BlockForSendingEmailUntil { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<CustomerEmailVerification> CustomerEmailVerifications { get; set; }
    }
}
