using Microsoft.AspNetCore.Http;
using Ecommerce.DTO.Application;
using Ecommerce.DTO.Customers.Apps;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Settings.Files;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Applications.ApplicationVersions
{
    public class VerisonDto : BaseDto
    {

    }
    #region API
    public class CreateVersionAPIInputDto : VerisonDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string ReleaseNumber { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsActive { get; set; }
        public int ApplicationId { get; set; }
        public bool IsHighlightedVersion { get; set; }
        public IFormFile File { get; set; }
        public string ProductCrmId { get; set; }
    }
    public class UpdateVersionAPIInputDto : CreateVersionAPIInputDto
    {
        public int Id { get; set; }

    }
    #endregion
    #region Input

    public class CreateVersionInputDto : VerisonDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        [JsonIgnore]
        public int ImageId { get; set; }
        public string MainPageUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string ReleaseNumber { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsActive { get; set; }
        public int ApplicationId { get; set; }
        public bool IsHighlightedVersion { get; set; }
        public string ProductCrmId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public FileDto File { get; set; }



    }
    public class UpdateVersionInputDto : CreateVersionInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class DeleteVersionInputDto : BaseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetVersionInputDto : BaseDto
    {
        public int Id { get; set; }
        // public int ApplicationId { get; set; }

    }
    #endregion

    #region Output
    public class GetVersionOutputDto : VerisonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }

        public string ShortDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsHighlightedVersion { get; set; }
        public string LongDescription { get; set; }

        public string ProductCRMId { get; set; }

        public FileStorageDto Logo { get; set; }
        public GetApplicationOutputDto Application { get; set; }
        [JsonIgnore]
        public VersionReleaseOutputDto VersionRelease { get; set; }
        public string DownloadUrl => VersionRelease?.DownloadUrl;
        public string ReleaseNumber => VersionRelease?.ReleaseNumber;
    }
    public class GetVersionDataOutputDto : VerisonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string MainPageUrl { get; set; }

        public string ShortDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsHighlightedVersion { get; set; }
        public string LongDescription { get; set; }
        public FileStorageDto Logo { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationTitle { get; set; }
        public int SubscriptionTypeId { get; set; }
        public VersionPriceDto MinVersionPrice { get; set; }
        [JsonIgnore]
        public VersionReleaseOutputDto VersionRelease { get; set; }
        public string DownloadUrl => VersionRelease?.DownloadUrl;
        public string ReleaseNumber => VersionRelease?.ReleaseNumber;
        public List<GetVersionPriceOutputDto> VersionPrices { get; set; }
        public IEnumerable<VersionFeaturesDto> VersionFeatures { get; set; } = new List<VersionFeaturesDto>();
        public List<GetApplicationPackagesOutputDto> Packages { get; set; } = new();

        public bool NotAvailable { get; set; }

    }

    #endregion

}
