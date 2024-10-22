using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class SubscriptionType:EntityBase
    {
        public SubscriptionType()
        {
            Applications = new HashSet<Application>();
            CustomerSubscriptions = new HashSet<CustomerSubscription>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Application> Applications { get; set; }
        public virtual ICollection<CustomerSubscription> CustomerSubscriptions { get; set; }
    }
}
