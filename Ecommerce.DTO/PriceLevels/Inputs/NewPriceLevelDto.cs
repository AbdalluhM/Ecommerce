using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Lookups.PriceLevels.Inputs
{
    public class NewPriceLevelDto : PriceLevelBaseDto
    {
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
}
