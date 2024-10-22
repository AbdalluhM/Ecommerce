using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ViewAccount:EntityBase
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountEmail { get; set; }
        public string AccountMobile { get; set; }
        public Guid? CountryId { get; set; }
        public string CountryName { get; set; }
        public Guid? AccountStatusId { get; set; }
        public string AccountStatusName { get; set; }
        public Guid ContractId { get; set; }
        public string ContractSerial { get; set; }
        public int? ContractStatusId { get; set; }
        public string ContractStatusName { get; set; }
    }
}
