using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Settings.Files;

namespace Ecommerce.DTO.Applications.ApplicationBase.Outputs
{
    public class RelatedAppDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public FileStorageDto Logo { get; set; } = new();

        public bool IsInWishlist { get; set; }

        public string FeaturedTag { get; set; }
        
        public bool HasLabel => Label != null ? true : false;

        public GetApplicationLabelOutputDto Label { get; set; } = new();

        public decimal Rate { get; set; }

        public int ReviewersCount { get; set; }

        public VersionPriceDetailsDto Price { get; set; } = new();
    }
}
