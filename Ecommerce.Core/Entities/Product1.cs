using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Product1:EntityBase
    {
        public Product1()
        {
            ContractDetails = new HashSet<ContractDetail>();
            Licens = new HashSet<Licens>();
        }

        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ContractDetail> ContractDetails { get; set; }
        public virtual ICollection<Licens> Licens { get; set; }
    }
}
