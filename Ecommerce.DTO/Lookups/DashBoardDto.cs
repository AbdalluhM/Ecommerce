namespace Ecommerce.DTO.Lookups
{

    public class GetDashBoardOutputDto : BaseDto
    {
        public int? TagsCount { get; set; } = 0;
        public int? PriceLevelsCount { get; set; } = 0;
        public int? AddOnsCount { get; set; } = 0;
        public int? FeaturesCount { get; set; } = 0;
        public int? CountriesCount { get; set; } = 0;
        public int? ApplicationsCount { get; set; } = 0;
        public int? ModulesCount { get; set; } = 0;
        public int TaxesCount { get; set; } = 0;



    }
}
