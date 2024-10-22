using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Tags;

using System.Collections.Generic;

namespace Ecommerce.DTO.Addons.AddonBase.Outputs
{
    public class AddonDetailsDto : BaseDto
    {
        public string Title { get; set; }

        public FileStorageDto Logo { get; set; }

        public string Description { get; set; }

        //
        public string ShortDescription { get; set; }

        public bool IsInWishlist { get; set; }

        //public AddonPriceDetailsDto AddonPrice { get; set; } = new();

        public AddonLabelDto AddonLabel { get; set; } = new();

        public List<string> SlidersPath { get; set; } = new();

        public List<GetTagOutputDto> Tags { get; set; } = new List<GetTagOutputDto>();

        public List<AppVersionDto> AvailableAppVersions { get; set; } = new List<AppVersionDto>();

        public List<PurchasedVersionsDto> PurchasedVersions { get; set; } = new List<PurchasedVersionsDto>();
    }

    public class AddOnPriceAllDetailsDto : VersionPriceAllDetailsDto
    {
        public int PriceLevelId { get; set; }
        public int AddOnPriceId { get; set; }
        //public int VersionAddOnId { get; set; }

    }
    public class AddOnPriceData
    {

        public AddOnPriceAllDetailsDto Price { get; set; }
    }


}
