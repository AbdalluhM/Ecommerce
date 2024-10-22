using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AddonSubscription:EntityBase
    {
        public int Id { get; set; }
        public int CustomerSubscriptionId { get; set; }
        public int VersionSubscriptionId { get; set; }
        public string AddonName { get; set; }
        public int AddonPriceId { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual AddOnPrice AddonPrice { get; set; }
        public virtual CustomerSubscription CustomerSubscription { get; set; }
        public virtual VersionSubscription VersionSubscription { get; set; }
    }
}
