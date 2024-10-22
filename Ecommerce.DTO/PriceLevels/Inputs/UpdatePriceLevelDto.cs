using System.Text.Json.Serialization;
using System;

namespace Ecommerce.DTO.Lookups.PriceLevels.Inputs
{
    public class UpdatePriceLevelDto : PriceLevelBaseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }

    public class GetPriceLevelInputDto : PriceLevelBaseDto
    {
        public int Id { get; set; }


    }
}
