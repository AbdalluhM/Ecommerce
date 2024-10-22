using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ViewDashboardTotal:EntityBase
    {
        public int? TagsCount { get; set; }
        public int? PriceLevelsCount { get; set; }
        public int? AddOnsCount { get; set; }
        public int? FeaturesCount { get; set; }
        public int? CountriesCount { get; set; }
        public int? ApplicationsCount { get; set; }
        public int? ModulesCount { get; set; }
        public int? TaxesCount { get; set; }
    }
}
