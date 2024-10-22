using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CountryCurrency:EntityBase
    {
        public CountryCurrency()
        {
            AddOnPrices = new HashSet<AddOnPrice>();
            EmployeeCountries = new HashSet<EmployeeCountry>();
            VersionPrices = new HashSet<VersionPrice>();
        }

        public int Id { get; set; }
        public int CountryId { get; set; }
        public int CurrencyId { get; set; }
        public bool DefaultForOther { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Country Country { get; set; }
        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual ICollection<AddOnPrice> AddOnPrices { get; set; }
        public virtual ICollection<EmployeeCountry> EmployeeCountries { get; set; }
        public virtual ICollection<VersionPrice> VersionPrices { get; set; }
    }
}
