using System.ComponentModel;

namespace Ecommerce.DTO.Lookups.PriceLevels
{
    public class PriceLevelBaseDto : BaseDto
    {
        public string Name { get; set; }
        public int NumberOfLicenses { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
