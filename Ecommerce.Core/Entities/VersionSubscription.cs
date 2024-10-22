using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class VersionSubscription:EntityBase
    {
        public VersionSubscription()
        {
            AddonSubscriptions = new HashSet<AddonSubscription>();
            Workspaces = new HashSet<Workspace>();
        }

        public int Id { get; set; }
        public int CustomerSubscriptionId { get; set; }
        public int VersionReleaseId { get; set; }
        public int VersionPriceId { get; set; }
        public string VersionName { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual CustomerSubscription CustomerSubscription { get; set; }
        public virtual VersionPrice VersionPrice { get; set; }
        public virtual VersionRelease VersionRelease { get; set; }
        public virtual ICollection<AddonSubscription> AddonSubscriptions { get; set; }
        public virtual ICollection<Workspace> Workspaces { get; set; }
    }
}
