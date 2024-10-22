using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CustomerCard:EntityBase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int PaymentMethodId { get; set; }
        public string CardNumber { get; set; }
        public string CardToken { get; set; }
        public string ExtraInfo { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDefault { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}
