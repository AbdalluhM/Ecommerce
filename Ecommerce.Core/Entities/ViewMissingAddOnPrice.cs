using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ViewMissingAddOnPrice:EntityBase
    {
        public string CountryName { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyShortCode { get; set; }
        public int CountryCurrencyId { get; set; }
        public int PriceLevelId { get; set; }
        public string PriceLevelName { get; set; }
        public int NumberOfLicenses { get; set; }
        public int AddOnId { get; set; }
        public int Id { get; set; }
    }
}
