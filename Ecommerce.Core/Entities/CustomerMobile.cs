using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CustomerMobile:EntityBase
    {
        public CustomerMobile()
        {
            CustomerSmsverifications = new HashSet<CustomerSmsverification>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Mobile { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public int SendCount { get; set; }
        public DateTime? BlockForSendingSmsuntil { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? PhoneCountryId { get; set; }
        public string PhoneCode { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Country PhoneCountry { get; set; }
        public virtual ICollection<CustomerSmsverification> CustomerSmsverifications { get; set; }
    }
}
