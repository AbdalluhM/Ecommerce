using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CustomerSubscription:EntityBase
    {
        public CustomerSubscription()
        {
            AddonSubscriptions = new HashSet<AddonSubscription>();
            Invoices = new HashSet<Invoice>();
            Licenses = new HashSet<License>();
            VersionSubscriptions = new HashSet<VersionSubscription>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public bool IsAddOn { get; set; }
        public int SubscriptionTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public string CurrencyName { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public int RenewEvery { get; set; }
        public int NumberOfLicenses { get; set; }
        public bool AutoBill { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual SubscriptionType SubscriptionType { get; set; }
        public virtual ICollection<AddonSubscription> AddonSubscriptions { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<License> Licenses { get; set; }
        public virtual ICollection<VersionSubscription> VersionSubscriptions { get; set; }
    }
}
