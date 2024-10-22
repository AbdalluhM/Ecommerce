using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CustomerStatu:EntityBase
    {
        public CustomerStatu()
        {
            Customers = new HashSet<Customer>();
        }

        public int Id { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
