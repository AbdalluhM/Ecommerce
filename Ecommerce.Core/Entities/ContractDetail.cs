using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ContractDetail:EntityBase
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public Guid? ProductId { get; set; }
        public int? NumberOfLicense { get; set; }
        public decimal? Price { get; set; }
        public int? StatusId { get; set; }

        public virtual Contract1 Contract { get; set; }
        public virtual Product1 Product { get; set; }
        public virtual ContractDetailsStatu Status { get; set; }
    }
}
