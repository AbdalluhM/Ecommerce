using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class PaymentType:EntityBase
    {
        public PaymentType()
        {
            PaymentMethods = new HashSet<PaymentMethod>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }
    }
}
