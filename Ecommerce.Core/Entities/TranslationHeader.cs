using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class TranslationHeader:EntityBase
    {
        public TranslationHeader()
        {
            Products = new HashSet<Product>();
            TranslationDetails = new HashSet<TranslationDetail>();
        }

        public int Id { get; set; }
        public int? TableId { get; set; }

        public virtual TableName Table { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<TranslationDetail> TranslationDetails { get; set; }
    }
}
