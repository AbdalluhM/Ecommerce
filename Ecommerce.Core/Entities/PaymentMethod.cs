using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class PaymentMethod:EntityBase
    {
        public PaymentMethod()
        {
            CountryPaymentMethods = new HashSet<CountryPaymentMethod>();
            CustomerCards = new HashSet<CustomerCard>();
            Invoices = new HashSet<Invoice>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int PaymentTypeId { get; set; }
        public string Credential { get; set; }
        public DateTime CreateDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDefault { get; set; }
        public bool CalcPaymentFee { get; set; }
        public bool IsCreatedBySystem { get; set; }
        public bool IsRefundMethod { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual PaymentType PaymentType { get; set; }
        public virtual ICollection<CountryPaymentMethod> CountryPaymentMethods { get; set; }
        public virtual ICollection<CustomerCard> CustomerCards { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
