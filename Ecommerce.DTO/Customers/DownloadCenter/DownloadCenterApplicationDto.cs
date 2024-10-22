using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Applications.ApplicationLabels;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Settings.Files;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Customers.DownloadCenter
{

    public class DownloadCenterResultDto
    {
        public IEnumerable<DownloadCenterApplicationResultDto> Applications { get; set; }
        public IEnumerable<DownloadCenterAddonResultDto> AddOns { get; set; }

    }
    public class DownloadCenterApplicationResultDto
    {
        public int Id { get; set; }
        public int subscribtionTypeId { get; set; }
        // -Name: display application name
        public string Name { get; set; }
        public string Title { get; set; }
        //public string Label { get; set; }
        public bool HasLabel => Label != null ? true : false;
        public GetApplicationLabelOutputDto Label { get; set; }
        public string Description { get; set; }
        public RateDto Rate { get; set; }
        public int DeviceTypeId { get; set; }
        public DownloadPriceDto Price { get; set; }
        public string CurrencyCode { get; set; }
        public FileStorageDto Logo { get; set; }
        public bool IsService { get; set; }
    }
    public class DownloadPriceDto
    {
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public decimal ForeverPrice { get; set; }
    }
    public class DownloadCenterAddonResultDto
    {
        public int Id { get; set; }
        public IEnumerable<int> ApplicationId { get; set; }
        // -Name: display application name
        public string Name { get; set; }
        public string Title { get; set; }
        //public string Label { get; set; }
        public bool HasLabel => Label != null ? true : false;
        public GetAddonLabelOutputDto Label { get; set; }
        public string Description { get; set; }
        public DownloadPriceDto Price { get; set; }
        public string CurrencyCode { get; set; }

        public FileStorageDto Logo { get; set; }
    }

    public class GetDownloadCenterApplicationsOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }
        public int SubscriptionTypeId { get; set; }
        public string SubscriptionType { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }
        public FileStorageDto Logo { get; set; }
        public RateDto Rate { get; set; }
        public List<GetDownloadCenterApplicationVersionOutputDto> Versions { get; set; }
    }

    public class GetDownloadCenterApplicationVersionOutputDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }

        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }

        public string ShortDescription { get; set; }
        public bool IsHighlightedVersion { get; set; }
        public string LongDescription { get; set; }
        public FileStorageDto Logo { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public VersionReleaseOutputDto VersionRelease { get; set; }
        public DateTime? LastUpdatedOn => VersionRelease?.CreateDate ?? CreateDate;

        public string DownloadUrl => VersionRelease?.DownloadUrl;
        public string ReleaseNumber => VersionRelease?.ReleaseNumber;
    }
}
