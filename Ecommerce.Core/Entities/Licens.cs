using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Licens:EntityBase
    {
        public int Id { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ContractId { get; set; }
        public string Serial { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? StatusId { get; set; }

        public virtual Contract1 Contract { get; set; }
        public virtual Product1 Product { get; set; }
        public virtual LicensesStatu Status { get; set; }
    }
}
