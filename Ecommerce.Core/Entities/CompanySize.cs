using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CompanySize:EntityBase
    {
        public CompanySize()
        {
            Customers = new HashSet<Customer>();
        }

        public int Id { get; set; }
        public string Size { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int? Crmid { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
