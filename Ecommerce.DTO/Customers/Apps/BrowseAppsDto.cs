using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Addons.AddonTags;
using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Applications.ApplicationTags;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Settings.Files;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Customers.Apps
{


    public class BrowseAppsOutputDto : BaseDto
    {
        public IEnumerable<BrowseAppsApplicationOutputDto> Applications { get; set; }
        public IEnumerable<BrowseAppsAddOnOutputDto> AddOns { get; set; }

    }
    public class BrowseAppsApplicationOutputDto : BaseDto
    {
        public int Id { get; set; }
        // -Name: display application name
        public string Name { get; set; }
        public string Title { get; set; }
        //public string Label { get; set; }
        public bool HasLabel => Label != null ? true : false;
        public GetApplicationLabelOutputDto Label { get; set; }
        public int SubscriptionTypeId { get; set; }
        public string SubscriptionType { get; set; }
        // -Description: display application short description
        public bool IsWishListed { get; set; } = false;
        public string Description { get; set; }
        [JsonIgnore]
        public string ShortDescription { get; set; }
        [JsonIgnore]

        public string LongDescription { get; set; }
        //-Image: display application image
        public FileStorageDto Image { get; set; }

        // -Featured Tag: tags from application tags
        public string FeaturedTag => ApplicationTags.Where(x => x.IsFeatured)?.FirstOrDefault()?.Name ?? string.Empty;
        [JsonIgnore]
        public List<GetApplicationTagOutputDto> ApplicationTags { get; set; }

        //public List<GetApplicationTagOutputDto> FeaturedTags { get; set; }
        // -Price: display application monthly price for customer country if exis
        //public decimal Price { get; set; }
        //public PriceDetailsDto Price { get; set; }
        public VersionPriceDetailsDto Price { get; set; }
        public VersionPriceAllDetailsDto PriceDetails { get; set; }
        public RateDto Rate { get; set; }
        //TODO:Update this dummy data
        //public decimal Rate { get; set; } 
        ////TODO:Update this dummy data

        //public int NumberOfRates { get; set; }

        public int DeviceTypeId { get; set; }

        public bool IsService { get; set; }


    }
    public class BrowseAppsAddOnOutputDto : BaseDto
    {
        public int Id { get; set; }
        public IEnumerable<int> ApplicationId { get; set; }
        // -Name: display application name
        public string Name { get; set; }
        public string Title { get; set; }
        //public string Label { get; set; }
        public bool HasLabel => Label != null ? true : false;
        public GetAddonLabelOutputDto Label { get; set; }
        //public int SubscriptionTypeId { get; set; }
        //public string SubscriptionType { get; set; }
        // -Description: display application short description
        public bool IsWishListed { get; set; } = false;
        public string Description { get; set; }
        [JsonIgnore]
        public string ShortDescription { get; set; }
        [JsonIgnore]

        public string LongDescription { get; set; }
        //-Image: display application image
        public FileStorageDto Image { get; set; }

        // -Features Tag: tags from application tags       
        public string FeaturedTag => AddOnTags.Where(x => x.IsFeatured)?.FirstOrDefault()?.Name ?? string.Empty;
        [JsonIgnore]
        public List<GetAddonTagOutputDto> AddOnTags { get; set; }

        // -Price: display application monthly price for customer country if exis
        public AddonPriceDetailsDto Price { get; set; }
        public VersionPriceAllDetailsDto PriceDetails { get; set; }

        //public decimal Rate { get; set; }
        //public int NumberOfRates { get; set; } 

    }
    //public class PriceDetailsDto
    //{
    //    public decimal PriceBeforeDiscount { get; set; }

    //    public decimal DiscountPercentage { get; set; }

    //    public decimal NetPrice { get; set; }

    //    public string CurrencySymbol { get; set; }

    //    public string Discrimination { get; set; } = "Monthly"; // TODO :should be more dynamic
    //}
}
