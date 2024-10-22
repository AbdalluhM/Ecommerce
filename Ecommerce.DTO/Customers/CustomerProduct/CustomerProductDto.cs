using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Addons.AddonLabels;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Tags;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Customers.CustomerProduct
{
    public partial class CustomerProductDto
    {
        #region Api
        public class CustomerProductApIInputDto
        {
            public int VersionSubscriptionId { get; set; }
        }

        #endregion
        public class AllLicencesFilterInputDto : FilteredResultRequestDto
        {
            public int CustomerId { get; set; }
            public int DeviceTypeId { get; set; }

        }
        public class LicencesFilterInputDto : FilteredResultRequestDto
        {
            [JsonIgnore]
            public int CustomerId { get; set; }

            public int VersionSubscriptionId { get; set; }
        }
        public class RequestChangeDeviceFilterDto : FilteredResultRequestDto
        {
            public int VersionSubscriptionId { get; set; }
        }
        public class CustomerProductInputDto
        {
            public int CustomerId { get; set; }

            public int VersionSubscriptionId { get; set; }
            public int AddOnSubscription { get; set; }

            [JsonIgnore]
            public int CountryId { get; set; }
        }
        public class VersionReleaseInputDto
        {
            [JsonIgnore]
            public int CustomerId { get; set; }

            public int VersionReleaseId { get; set; }
            public string IpAddress { get; set; }

        }
        public class RefundRequestInputDto
        {
            [JsonIgnore]
            public int CustomerId { get; set; }
            public string Reason { get; set; }
            [JsonIgnore]
            public DateTime CreateDate { get; set; }
            public int VersionSubscriptionId { get; set; }
        }

        public class CutomerSubscriptionInputDto
        {
            [JsonIgnore]
            public int CustomerId { get; set; }

            public int VersionSubscriptionId { get; set; }
        }
        public class UpdateCutomerSubscriptionInputDto
        {
            [JsonIgnore]
            public int CustomerId { get; set; }
            public bool AutoBill { get; set; }
            public int VersionSubscriptionId { get; set; }
        }
        #region Liceses
        public class DownloadFileActivationOutputDto
        {
            //Todo after put data in table
            public string ActivationFilePath { get; set; } = "sasajn fksjd";
            public DateTime KeyUploadedOn { get; set; } = DateTime.Now;
        }
        public class LicenseActionInputDto
        {
            [JsonIgnore]
            public int CustomerId { get; set; }
            public int VersionSubscriptionId { get; set; }
            public int LicenseId { get; set; }
        }
        public class AddOnLicenseActionInputDto
        {
            [JsonIgnore]
            public int CustomerId { get; set; }
            public int VersionSubscriptionId { get; set; }
            public int AddOnSubscriptionId { get; set; }
            public int LicenseId { get; set; }
        }
        public class CreateLicenseInputDto
        {
            [JsonIgnore]
            public int CustomerId { get; set; }
            public int VersionSubscriptionId { get; set; }
            public string DeviceName { get; set; }
            public string SerialNumber { get; set; }
        }
        public class UpdateLicenseInputDto : CreateLicenseInputDto
        {
            public int DeviceId { get; set; }
            public int ReasonChangeId { get; set; }
            [JsonIgnore]
            public DateTime CreateDate { get; set; }
        }
        #endregion

        public class AddOnSubscriptionIdsDto
        {
            public int AddOnSubscriptionId { get; set; }
            public int AddOnId { get; set; }
            public bool IsActive { get; set; }
        }
        public class GetAllCustomerProductAddonOutputDto
        {

            public bool IsActive { get; set; }
            public int AddonSubscriptionId { get; set; }

            public int AddOnId { get; set; }

            public int PriceLevelId { get; set; }

            public string AddOnName { get; set; }

            public string Description { get; set; }

            public FileStorageDto Image { get; set; }

            public bool IsPurshased { get; set; }

            public AddonPriceDetailsDto Price { get; set; }

            [JsonIgnore]
            public AddOnPriceAllDetailsDto AllPrices { get; set; }

            public GetAddOnPurshasedOutputDto PurshasedData { get; set; }

            public bool HasLabel => Label != null ? true : false;

            public GetAddonLabelOutputDto Label { get; set; } = null;

            public string FeaturedTag => Tag?.Name ?? string.Empty;

            [JsonIgnore]
            public GetTagOutputDto Tag { get; set; }

            public int VersionId { get; set; }
            [JsonIgnore]
            public int? PaymentCountryId { get; set; }
            public DateTime CreatedDate { get; set; }

            public bool CanCancle { get; set; }


        }

        public class GetAddOnPurshasedOutputDto
        {
            public int AddonSubscriptionId { get; set; }
            public int? StatusId { get; set; }
            public string Status { get; set; }
            public int RenwalEvery { get; set; }
            public DateTime? RenewalDate { get; set; }
            public DateTime? LicenseRenewalDate { get; set; }
            public FileStorageDto File { get; set; } = new();
        }

        public class GetAddOnPriceNotPurshased
        {
            public AddonPriceDetailsDto Price { get; set; }
        }
        public class GetCustomerProductLicensesOutputDto
        {

            public IEnumerable<GetLicenseOutputDto> Licences { get; set; }
        }
        public class GetReasonChangeDeviceOutputDto
        {
            public int Id { get; set; }
            public string Reason { get; set; }
        }

        public class GetLicenseOutputDto
        {
            public int Id { get; set; }
            public string DeviceName { get; set; }
            public string SerialNumber { get; set; }
            public DateTime? ActivatedOn { get; set; }
            public DateTime RenwalDate { get; set; }
            public int StatusId { get; set; }
            public int VersionSubscriptionId { get; set; }
            [JsonIgnore]
            public int CustomerSubscriptionId { get; set; }
            public string Status { get; set; }
            public bool HasRelatedAddonLicense { get; set; }
            public bool CanCreate { get; set; }
            public bool HasChangedRequest { get; set; }
            public bool PaiedStatus { get; set; }
            public FileStorageDto File { get; set; } = new();
        }

        public class GetRequestChangeDeviceOutputDto
        {
            public int Id { get; set; }
            public string OldDeviceName { get; set; }
            public string NewDeviceName { get; set; }
            public string OldSerial { get; set; }
            public string NewSerial { get; set; }
            public string Status { get; set; }
            public string Reason { get; set; }
            //public DateTime ModifiedDate { get; set; }
            public DateTime CreateDate { get; set; }
            public DateTime ActionDate { get; set; }
        }
        public class GetCustomerProductLicenseAddonOutputDto
        {
            public int Id { get; set; }
            public string DeviceName { get; set; }
            public string SerialNumber { get; set; }
        }
        public class GetCustomerSubscriptionOutputDto
        {
            public bool CanCancel { get; set; }
            public string CustomerName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool AutoBill { get; set; }
            public int Id { get; set; }
            public bool IsAddOn { get; set; }
            public string CurrencyName { get; set; }
            public decimal Price { get; set; }
            public decimal PriceAfterDiscount { get; set; }
            public int RenewEvery { get; set; }
            public int NumberOfLicenses { get; set; }
            public bool IsExpired { get; set; }
        }
        public class GetCustomerProductOutputDto
        {
            public int AppId { get; set; }
            public bool IsDefault { get; set; }
            public int VersionSubscriptionId { get; set; }
            public int CustomerSubscriptionId { get; set; }
            public int ProductId { get; set; }
            public int VersionReleaseId { get; set; }
            public string VersionName { get; set; }
            public string VersionDescription { get; set; }
            public string ApplicationName { get; set; }
            public int NumberOfLicenses { get; set; }
            public VersionPriceDetailsDto Price { get; set; }
            //Todo replace this data
            public int UsedDevice { get; set; }
            public FileStorageDto Logo { get; set; }
            //Todo
            public int NewRelease { get; set; }
            public string ReleaseNumber { get; set; }
            public string PriceLevel { get; set; }
            public int PriceLevelId { get; set; }
            public DateTime RenewDate { get; set; }
            public string Currency { get; set; }

        }
        public class GetCustomerApplicationVersionsAndAddOnsDto
        {
            public List<GetCustomerApplicationVersionsDto> GetCustomerApplicationVersionsDto { get; set; }
            public List<GetAllCustomerProductAddonOutputDto> GetAllCustomerProductAddonOutputDto { get; set; }
        }
        public class GetCustomerApplicationVersionsDto
        {
            public int Id { get; set; }
            public int? VersionSubscriptionId { get; set; }
            public int? LicenseCount { get; set; }
            public int NumberOfLicence { get; set; }
            public int SubscriptionType { get; set; }
            public DateTime? SubscriptionDate { get; set; }
            public DateTime? RenewDate { get; set; }
            public string Name { get; set; }
            public string ShortDescription { get; set; }
            public string LongDescription { get; set; }
            public bool Purshased { get; set; }
            public bool CanCreate { get; set; }
            public bool HasOpenRefundRequest { get; set; }
            public bool HasPaiedAddons { get; set; }
            public FileStorageDto Logo { get; set; }
            public VersionPriceDetailsDto MinVersionPrice { get; set; }
        }

        public class CustomerProductWorkspacesDto
        {
            public int Id { get; set; }
            public string VersionName { get; set; }
            public int UsedLicesesCount { get; set; }
            public int LicensesCount { get; set; }
            public DateTime NextRenewalDate { get; set; }
        }
        public class CustomerProductLookupDto
        {
            public string CrmId { get; set; }
            public string VersionName { get; set; }
            [JsonIgnore]
            public int VersionPriceId { get; set; }
            public int VersionSubscriptionId { get; set; }
            public bool CanCreate { get; set; }
            public bool PaiedStatus { get; set; }
        }

        public class GetReleasesOutputDto
        {
            [JsonIgnore]
            public int VersionId { get; set; }
            [JsonIgnore]
            public DateTime VersionDate { get; set; }
            public int Id { get; set; }
            public string VersionName { get; set; }
            public string Size { get; set; } = "200MB";
            public string ReleaseTitle { get; set; }
            public int LicenseCount { get; set; }
            public DateTime SubscriptionDate { get; set; }
            public DateTime RenewDate { get; set; }
            //ShortDescription
            public string Description { get; set; }

            public string DownloadUrl { get; set; }
            //Creation date
            public DateTime Released { get; set; }
            public bool CanDownload { get; set; }
            public bool IsLatest { get; set; }
            public bool IsDownload { get; set; }
            public bool IsCurrent { get; set; }
        }

        public class CustomerApplicationLookupDto
        {
            public int id { get; set; }
            public string Name { get; set; }

        }

        public class NearestVersionRenewDto
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }
            public DateTime NearestRenewDate { get; set; }
        }

    }
}
