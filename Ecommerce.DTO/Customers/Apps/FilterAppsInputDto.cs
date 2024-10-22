using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Customers.Apps
{
    public class FilterAppsInputDto
    {
        public int? SubscriptionTypeId { get; set; }
        public List<int>? TagIds { get; set; }
        public List<int>? ModuleIds { get; set; }

        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }

        public string SearchTerm { get; set; }
        [JsonIgnore]
        public int CurrentCustomerId { get; set; }
    }
    public class GetPriceRangeOutputDto
    {
       
        public decimal PriceFrom => Ranges.Min(x => x.PriceFrom);
        public decimal PriceTo => Ranges.Max(x => x.PriceTo);

        public IEnumerable<GetApplicationPriceRangeOutputDto> Ranges ;
    }

    public class GetApplicationPriceRangeOutputDto
    {
        public int ApplicationId { get; set; }
        [JsonIgnore]
        public decimal MinForeverPrice { get; set; }
        [JsonIgnore]
        public decimal MinYearlyPrice { get; set; }
        [JsonIgnore]
        public decimal MinMonthlyPrice { get; set; }
        [JsonIgnore]
        public decimal MaxForeverPrice { get; set; }
        [JsonIgnore]
        public decimal MaxYearlyPrice { get; set; }
        [JsonIgnore]
        public decimal MaxMonthlyPrice { get; set; }

      
        public decimal PriceFrom => Math.Min(MinMonthlyPrice, Math.Min(MinYearlyPrice, MinForeverPrice));
        public decimal PriceTo => Math.Max(MaxMonthlyPrice, Math.Max(MaxYearlyPrice, MaxForeverPrice));
    }
}
