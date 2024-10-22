using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CountryPaymentMethod:EntityBase
    {
        public int Id { get; set; }
        public int? CountryId { get; set; }
        public int PaymentMethodId { get; set; }

        public virtual Country Country { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}
