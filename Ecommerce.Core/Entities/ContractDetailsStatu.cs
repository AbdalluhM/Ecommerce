using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ContractDetailsStatu:EntityBase
    {
        public ContractDetailsStatu()
        {
            ContractDetails = new HashSet<ContractDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ContractDetail> ContractDetails { get; set; }
    }
}
