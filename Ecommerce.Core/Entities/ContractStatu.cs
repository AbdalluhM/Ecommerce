using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ContractStatu:EntityBase
    {
        public ContractStatu()
        {
            Contract1 = new HashSet<Contract1>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Contract1> Contract1 { get; set; }
    }
}
