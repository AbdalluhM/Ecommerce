using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class InvoiceDetail:EntityBase
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string VersionName { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Discount { get; set; }
        public decimal NetPrice { get; set; }
        public decimal? VatAmount { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
