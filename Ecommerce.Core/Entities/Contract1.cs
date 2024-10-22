using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Contract1:EntityBase
    {
        public Contract1()
        {
            ContractDetails = new HashSet<ContractDetail>();
            Licens = new HashSet<Licens>();
        }

        public Guid Id { get; set; }
        public string Serial { get; set; }
        public Guid AccountId { get; set; }
        public int? StatusId { get; set; }

        public virtual Account Account { get; set; }
        public virtual ContractStatu Status { get; set; }
        public virtual ICollection<ContractDetail> ContractDetails { get; set; }
        public virtual ICollection<Licens> Licens { get; set; }
    }
}
