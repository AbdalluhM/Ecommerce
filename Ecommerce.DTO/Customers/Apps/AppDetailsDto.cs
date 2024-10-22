using Ecommerce.DTO.Application;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Features;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Settings.Files;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Customers.Apps
{
    public class GetAppDetailsPricingAndPlansInputDto
    {
        public int ApplicationId { get; set; }
        public int CountryId { get; set; }
    }
    public class GetAvailableVersionsByApplicationAndPricingInputDto
    {
        public int ApplicationId { get; set; }
        public int? PriceLevelId { get; set; }
    }
    public class GetVersionForBuyInputDto
    {
        public int VersionId { get; set; }
        public int? PriceLevelId { get; set; }
    }
    public class GetAppDetailsPricingAndPlansOutputDto
    {
        public GetApplicationDropDownOutputDto Application { get; set; }
        // public IEnumerable<GetVersionsByApplicationAndPricingOutputDto> Versions { get; set; }
        public IEnumerable<GetApplicationPackagesOutputDto> Packages { get; set; }
        public IEnumerable<GetFeatureDropDownOutputDto> Features { get; set; }
    }
    public class PriceLevelResultDto
    {
        public int Id { get; set; }
        public int NumberOfLicences { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }

    public class GetVersionsByApplicationAndPricingOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ApplicationId { get; set; }
        public int SubscriptionTypeId { get; set; }
        public FileStorageDto Image { get; set; }
        public bool IsHighlightedVersion { get; set; }

        [JsonIgnore]

        public IEnumerable<GetVersionPriceOutputDto> VersionPrices { get; set; }
        [JsonIgnore]
        public VersionReleaseOutputDto VersionRelease { get; set; }
        public string ReleaseNumber => VersionRelease?.ReleaseNumber;
        public int VersionReleaseId => VersionRelease?.Id ?? 0;
        public VersionPriceAllDetailsDto Price { get; set; } = new VersionPriceAllDetailsDto();

        public IEnumerable<VersionFeaturesDto> AvailabeFeatures { get; set; } = new List<VersionFeaturesDto>();
        public int? PaymentCountryId => Price?.Monthly?.PaymentCountryId > 0 ? Price?.Monthly?.PaymentCountryId : Price?.Forever?.PaymentCountryId;

        public bool NotAvailable { get; set; }

    }


    public class VersionForBuyNowOutputDto
    {
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public string VersionTitle { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationTitle { get; set; }

        public FileStorageDto VersionImage { get; set; }



        public VersionPriceAllDetailsDto VersionPrice { get; set; } = new VersionPriceAllDetailsDto();


        public VersionReleaseOutputDto VersionRelease { get; set; }
        public string ReleaseNumber => VersionRelease?.ReleaseNumber;
        public string Discriminator { get; set; }
        public int SubscriptionTypeId { get; set; }

        //[JsonIgnore]
        //public VersionPriceDto MinVersionPrice { get; set; }
        //[JsonIgnore]
        //public IEnumerable<GetVersionPriceOutputDto> VersionPrices { get; set; }

        //public List<GetApplicationPackagesOutputDto> Packages { get; set; } = new();
    }
    public class GetApplicationPackagesOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfLicenses { get; set; }
        public bool IsDefault { get; set; }
        public GetSubscriptionTypeOutputDto SubscriptionType => Details?.FirstOrDefault()?.SubscriptionType;

        public IEnumerable<GetApplicationPackageDetailsoutputDto> Details { get; set; } = new List<GetApplicationPackageDetailsoutputDto>();
    }
    public class GetApplicationPackageDetailsoutputDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int VersionId { get; set; }
        public int VersionReleaseId { get; set; }

        [JsonIgnore]
        public GetSubscriptionTypeOutputDto SubscriptionType { get; set; }

        public VersionPriceAllDetailsDto Price { get; set; }

        [JsonIgnore]
        public VersionPriceDto MinPrice { get; set; }




    }
}
