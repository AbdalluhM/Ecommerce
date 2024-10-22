using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class OauthType:EntityBase
    {
        public OauthType()
        {
            Customers = new HashSet<Customer>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
