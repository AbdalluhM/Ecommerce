using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class VersionPrice:EntityBase
    {
        public VersionPrice()
        {
            VersionSubscriptions = new HashSet<VersionSubscription>();
        }

        public int Id { get; set; }
        public int VersionId { get; set; }
        public int CountryCurrencyId { get; set; }
        public int PriceLevelId { get; set; }
        public decimal YearlyPrice { get; set; }
        public decimal YearlyPrecentageDiscount { get; set; }
        public decimal YearlyNetPrice { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal MonthlyPrecentageDiscount { get; set; }
        public decimal MonthlyNetPrice { get; set; }
        public decimal ForeverPrice { get; set; }
        public decimal ForeverPrecentageDiscount { get; set; }
        public decimal ForeverNetPrice { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual CountryCurrency CountryCurrency { get; set; }
        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual PriceLevel PriceLevel { get; set; }
        public virtual Version Version { get; set; }
        public virtual ICollection<VersionSubscription> VersionSubscriptions { get; set; }
    }
}
