using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Settings.Files;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;

namespace Ecommerce.DTO.Customers.Invoices.Outputs
{
    #region Input

    public class GetInvoiceInputDto
    {
        public int Id { get; set; }
    }

    public class APIPaymentDetailsInputDto
    {
        public int PaymentCountryId { get; set; }
        public int VersionId { get; set; }
        public int? AddOnId { get; set; }
        public int PriceLevelId { get; set; }
        public int Discriminator { get; set; } = 0;// ForEver = 1,Monthly =2 ,Yearly = 3

        public int VersionSubscriptionId { get; set; } // for AddOn


    }
    public class APIInvoicePaymentDetailsInputDto
    {
        public int InvoiceId { get; set; }
        //[JsonIgnore]
        //public int CustomerId { get; set; }
        [JsonIgnore]
        public int CountryId { get; set; }
        [JsonIgnore]
        public int Discriminator { get; set; } = 0;// ForEver = 1,Monthly =2 ,Yearly = 3
        [JsonIgnore]
        public bool IsAddOn { get; set; }

    }
    public class PaymentDetailsInputDto
    {
        public int InvoiceId { get; set; }
        public int VersionId { get; set; }
        public int? AddOnId { get; set; }

        public int PriceLevelId { get; set; }
        public int Discriminator { get; set; } // ForEver = 1,Monthly =2 ,Yearly = 3
        [JsonIgnore]
        public int CustomerId { get; set; }
        [JsonIgnore]
        public int CountryId { get; set; }
        [JsonIgnore]
        public bool IsAddOn => AddOnId.HasValue && AddOnId.Value > 0;

        public int VersionReleaseId { get; set; }

        [JsonIgnore]
        public bool IsFawryCard { get; set; } = false;
        [JsonIgnore]
        public bool IsUnPaid { get; set; } = true;
        [JsonIgnore]
        public int PaymentMethodId { get; set; }
        [JsonIgnore]
        public int InvoiceStatusId { get; set; }
        [JsonIgnore]
        public int InvoiceTypeId { get; set; }
        public bool IsInvoiceSupportAdmin { get; set; }
        public bool IsProductInvoice { get; set; }
        public bool IsAdminInvoice { get; set; }
        public string InvoiceTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public int CustomerSubscriptionId { get; set; }
        public int VersionSubscriptionId { get; set; }
        public int? AddOnSubscriptionId { get; set; }
        //public bool AutoBill { get; set; }
        [JsonIgnore]
        public string? InvoiceSerial { get; set; }

        public int AdminInvoiceTypeId { get; set; }
    }
    public class InvoicePaymentDetailsInputDto
    {
        public int InvoiceId { get; set; }
        //[JsonIgnore]
        //public int CustomerId { get; set; }
        [JsonIgnore]
        public int CountryId { get; set; }
        [JsonIgnore]
        public int Discriminator { get; set; } = 0;// ForEver = 1,Monthly =2 ,Yearly = 3
        [JsonIgnore]
        public bool IsAddOn { get; set; }


    }
    public class GetPaymentTypesInputDto
    {
        public int CustomerId { get; set; }

    }
    #endregion
    #region Output

    //public class CreateInvoiceOutputDto
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Title { get; set; }
    //    public int ApplicationId { get; set; }
    //    public FileStorageDto Image { get; set; }
    //    public bool IsHighlightedVersion { get; set; }
    //    [JsonIgnore]
    //    public VersionReleaseOutputDto VersionRelease { get; set; }
    //    [JsonIgnore]

    //    public IEnumerable<GetVersionPriceOutputDto> VersionPrices { get; set; }

    //    public string ReleaseNumber => VersionRelease?.ReleaseNumber;
    //    public VersionPriceAllDetailsDto Price { get; set; } = new VersionPriceAllDetailsDto();
    //}




    #region Payments
    public class GetPayAndSubscribeOutputDto : APIPayAndSubscribeInputDto
    {
        [JsonIgnore]
        public InvoicePaymentInfoJson PaymentResult { get; set; }

    }
    public class APIPayAndSubscribeInputDto
    {

        public int PriceLevelId { get; set; }
        public string PriceLevelName { get; set; }
        public VersionPriceAllDetailsDto VersionPrice { get; set; } = new VersionPriceAllDetailsDto();
        public string Title => IsAddOn ? AddonTitle : ApplicationTitle;
        public FileStorageDto Image { get; set; }

        public int ApplicationId { get; set; }
        public string ApplicationTitle { get; set; }
        [JsonIgnore]
        public int Id { get; set; }
        public int CustomerSubscriptionId { get; set; }
        public int? VersionSubscriptionId { get; set; }
        public int? AddOnSubscriptionId { get; set; }
        [JsonIgnore]
        public int CustomerId { get; set; }
        [JsonIgnore]
        public GetCustomerOutputDto Customer { get; set; }

        public int? VersionId { get; set; }
        public int? AddOnId { get; set; }

        public int VersionReleaseId { get; set; }
        public string VersionTitle { get; set; }
        public string AddonTitle { get; set; }

        public int? VersionPriceId { get; set; }

        public int AddonPriceId { get; set; }

        public bool IsAddOn => AddOnId.HasValue && AddOnId.Value > 0 ? true : false;




        public string SuccessCallBackUrl { get; set; }
        public string FailCallBackUrl { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int InvoiceStatusId { get; set; } = (int)InvoiceStatusEnum.Unpaid;
        public int InvoiceTypeId { get; set; }
        public int PaymentMethodId { get; set; }
        //public string TaxReg { get; set; }
        //public string Address { get; set; }
        public int InvoiceId { get; set; }
        [JsonIgnore]
        public DateTime PaymentDate { get; set; }
        [JsonIgnore]
        public DateTime StartDate { get; set; }
        [JsonIgnore]
        public DateTime EndDate { get; set; }
        public string PaymentInfo { get; set; }
        [JsonIgnore]
        public string Notes { get; set; }
        [JsonIgnore]
        public string Serial { get; set; } = "INV_SER";
        public decimal SubTotal { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal Total { get; set; }
        public int SubscriptionTypeId { get; set; }
        public string CurrencyName { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public int RenewEvery { get; set; }
        public int NumberOfLicenses { get; set; }
        public bool AutoBill { get; set; } = true;
        public bool IsFawryCard { get; set; } = false;
        public string CardToken { get; set; }
        //[JsonIgnore]
        public List<CreateInvoiceDetailInputDto> Details { get; set; }
        [JsonIgnore]
        public PaymentTypeDto PaymentType { get; set; }
        [JsonIgnore]
        public bool IsUnPaid { get; set; } = false;

        public string InvoiceTitle { get; set; }
        [JsonIgnore]
        public bool IsAdminInvoice { get; set; } = false;

        public string Url { get; set; }
        //[JsonIgnore]
        //public bool IsProductInvoice { get; set; } = false;
        [JsonIgnore]
        public string? InvoiceSerial { get; set; }
        public string VatId { get; set; }
    }


    public class UpdateInvoicePaymentStatusInputDto
    {
        public int InvoiceId { get; set; }
        public int PaymentStatusId { get; set; }
    }
    public class PayAndSubscribeInputDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int VersionId { get; set; }
        public int? AddOnId { get; set; }
        public int CustomerSubscriptionId { get; set; }
        public int VersionSubscriptionId { get; set; }
        public int? AddOnSubscriptionId { get; set; }
        public string SuccessCallBackUrl { get; set; }
        public string FailCallBackUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public InvoiceDto Invoice { get; set; }
        public CreateSubscriptionInputDto Subscription { get; set; }
    }
    #endregion
    #region Customer Subscription


    #endregion
    #region Version Subscription  


    #endregion
    #region AddOn Subscription

    #endregion
    #region License
    #endregion
    #region Invoice
    #endregion
    #region Invoice Details
    #endregion
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string Serial { get; set; }
        public int CustomerSubscriptionId { get; set; }
        [JsonIgnore]
        public int InvoiceStatusId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int PaymentMethodId { get; set; }
        public string TaxReg { get; set; }
        public string Address { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentInfo { get; set; }
        public string Notes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal Total { get; set; }
        public DateTime CreateDate { get; set; }
        public List<CreateInvoiceDetailInputDto> Details { get; set; }

    }

    public class InvoiceTotals
    {
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal Total { get; set; }

        public List<CreateInvoiceDetailInputDto> Details { get; set; }


    }
    public class CreateInvoiceDetailInputDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string VersionName { get; set; }

        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Discount { get; set; }
        public decimal NetPrice { get; set; }
        public decimal? VatAmount { get; set; }
    }
    public class ThirdPartyInvoiceInputDto
    {
        public int PaymentMethodId { get; set; }
        public int CustomerId { get; set; }
        public string CurrencyCode { get; set; }
        public string SuccessCallBackUrl { get; set; }
        public string FailCallBackUrl { get; set; }
        public bool IsFawryCard { get; set; } = false;
        public string CardToken { get; set; }
        public PaymentTypeDto PaymentType { get; set; }
        public GetCustomerOutputDto Customer { get; set; }

    }
    #region Payment GateWay Settings


    //public class FawryInitializeDto
    //{
    //    public static string BaseUrl => "https://atfawry.fawrystaging.com/";

    //    public static string MerchantCode => "siYxylRjSPxNlGqLQUaqSg==";//"siYxylRjSPxOhfMQ50ppCw==";
    //    public static string SecurityKey => "c46915b6-28ff-4081-8395-3bdc2d0ad865";//"7b6d6829-b50e-4e08-aa67-296c8d2185f8";
    //}

    //public class PayPalInitializeDto
    //{
    //    public static string BaseUrl => "https://api-m.sandbox.paypal.com";

    //    public static string ClientId => "AU8J2-a8iRJElGIARvK2P8tNntgxjW2rpeS8OwF00Uj0S4cadGooSRVa2XNPFJHmVX_kVAWOGlFOeWej";
    //    public static string ClientSecret => "ECTdvFmqgPf4j8BsYJGgBERh06qfp7QUEtnhws6YjIK5h9gqQRMO09nxCoRPfRx5P9kSoSQANcU7g1dW";
    //}

    public class PaymentGateWaySettings
    {
        public string Language { get; set; }
        public bool IsSandBox { get; set; }
        public PayPalSettings PayPal { get; set; }
        public FawrySettings Fawry { get; set; }
        public BankSettings Bank { get; set; }
        public CurrencyConversionSettings Currency { get; set; }

    }
    public class PayPalSettings
    {
        public string BaseUrl { get; set; }
        //public string ClientId { get; set; }
        //public string ClientSecret { get; set; }
        public string BaseCurrency { get; set; }
    }


    public class FawrySettings
    {
        public string BaseUrl { get; set; }

        //public string MerchantCode { get; set; }
        //public string SecurityKey { get; set; }
        public string BaseCurrency { get; set; }
        public int PaymentExpirationDays { get; set; }

    }
    public class BankSettings
    {
        //public string AccessCode { get; set; }
        //public string MerchantIdentifier { get; set; }
        public string SHAType { get; set; }
        public string SHARequestPhrase { get; set; }
        public string SHAResponsePhrase { get; set; }
        //public int DefaultMultiplyFactor { get; set; }

    }

    public class CurrencyConversionSettings
    {
        public string BaseApiUrl { get; set; }
        public string ApiKey { get; set; }
    }
    #endregion
    public class CreateSubscriptionInputDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public bool IsAddOn => Version != null ? false : true;
        public int SubscriptionTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public string CurrencyName { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public int RenewEvery { get; set; }
        public int NumberOfLicenses { get; set; }
        public bool AutoBill { get; set; }

        public VersionSubscriptionInputDto Version { get; set; }
        public AddOnSubscriptionInputDto AddOn { get; set; }
    }
    public class VersionSubscriptionInputDto
    {
        public string Serial { get; set; }
        public int CustomerSubscriptionId { get; set; }
        public int InvoiceStatusId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int PaymentMethodId { get; set; }
        public string TaxReg { get; set; }
        public string Address { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentInfo { get; set; }
        public string Notes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal Total { get; set; }
        public DateTime CreateDate { get; set; }
    }
    public class AddOnSubscriptionInputDto
    {
        public int CustomerSubscriptionId { get; set; }
        public int VersionSubscriptionId { get; set; }
        public int AddonPriceId { get; set; }
        public DateTime CreateDate { get; set; }
    }
    #endregion
    public class GetSubscriptionStatusDto
    {
        public int CustomerSubscriptionId { get; set; }
        public bool IsValid { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
