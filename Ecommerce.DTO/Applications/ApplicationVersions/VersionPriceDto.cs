using Ecommerce.Core.Entities;
using Ecommerce.DTO.Lookups;

using System;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Applications.ApplicationVersions
{

    public class VersionPriceDto : BaseDto
    {
        public int VersionId { get; set; }
        public int CountryCurrencyId { get; set; }
        public int PriceLevelId { get; set; }
    }

    #region Input
    public class CreateVersionPriceInputDto : VersionPriceDto
    {
        public decimal YearlyPrice { get; set; }
        public decimal YearlyPrecentageDiscount { get; set; }
        public decimal YearlyNetPrice
        {
            get
            {
                return YearlyPrice - (YearlyPrice * YearlyPrecentageDiscount / 100);
            }
            set { }
        }
        public decimal MonthlyPrice { get; set; }
        public decimal MonthlyPrecentageDiscount { get; set; }
        public decimal MonthlyNetPrice
        {
            get
            {
                return MonthlyPrice - (MonthlyPrice * MonthlyPrecentageDiscount / 100);
            }
            set { }
        }
        public decimal ForeverPrice { get; set; }
        public decimal ForeverPrecentageDiscount { get; set; }
        public decimal ForeverNetPrice
        {
            get
            {
                return ForeverPrice - (ForeverPrice * ForeverPrecentageDiscount / 100);
            }
            set { }
        }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }


    }

    public class UpdateVersionPriceInputDto : CreateVersionPriceInputDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class DeleteVersionPriceInputDto : BaseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
    public class GetVersionPriceInputDto : BaseDto
    {
        public int Id { get; set; }

    }
    public class GetVersionPricesInputDto : BaseDto
    {
        public int VersionId { get; set; }

    }
    public class GetApplicationVersionPricesInputDto : BaseDto
    {
        public int ApplicationId { get; set; }

    }


    #endregion
    #region Output
    public class GetVersionPriceOutputDto : UpdateVersionPriceInputDto
    {
        public string CountryName { get; set; }
        public string CurrencyShortCode { get; set; }
        public string PriceLevelName { get; set; }
        public string NumberOfLicenses { get; set; }
        public string VersionName { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int SubscriptionTypeId { get; set; }

    }
    public class PurchasedVersionsDto
    {
        [JsonIgnore]
        public VersionSubscription VersionSubscription { get; set; }
        public int VersionSubscriptionId => VersionSubscription != null ? VersionSubscription.Id : 0;

        public int VersionPriceId { get; set; }
        public int VersionReleaseId { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public int UsedLicensesCount { get; set; }
        public int LicensesCount { get; set; }

        public DateTime NextRenewDate { get; set; }
        public int PriceLevelId { get; set; }
        public string PriceLevelName { get; set; }
        public int VersionAddOnId { get; set; }
        public int AddOnPriceId { get; set; }
        public int Discrimnator { get; set; }
        public GetSubscriptionTypeOutputDto Subscription { get; set; }

        public bool IsDefault => AddOnPriceId > 0 ? true : false;
       // public bool IsAssigned { get; set; }

    }
    #endregion
}