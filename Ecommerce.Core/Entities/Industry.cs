using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Industry:EntityBase
    {
        public Industry()
        {
            Customers = new HashSet<Customer>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Isactive { get; set; }
        public DateTime CreateDate { get; set; }
        public int? Crmid { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
