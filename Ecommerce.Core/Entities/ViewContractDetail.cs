using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ViewContractDetail:EntityBase
    {
        public Guid ContractId { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public int? NumberOfLicenses { get; set; }
        public decimal? Price { get; set; }
        public int? ContractDetailStatusId { get; set; }
        public string ContractDetailStatusName { get; set; }
    }
}
