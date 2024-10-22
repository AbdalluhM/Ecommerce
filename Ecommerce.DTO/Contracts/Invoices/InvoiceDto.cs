using Ecommerce.DTO.Addons.AddonBase.Outputs;
using Ecommerce.DTO.Applications.ApplicationVersions;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Lookups.PriceLevels.Outputs;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.DTO.Taxes;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Contracts.Invoices
{
    public class InvoiceDto
    {
        public class CancelInvoiceInputDto
        {
            public string CancelReason { get; set; }
            public List<int> ids { get; set; }
        }
        public class GetSupportInvoicePriceOutputDto
        {
            public decimal SupportPrice { get; set; }
            public decimal VatPrice { get; set; }
            public decimal NetPrice { get; set; }
            public decimal TotalPrice { get; set; }
        }
        public class CreateAdminInvoiceInputDto
        {
            public int VersionId { get; set; }
            public int? AddOnId { get; set; }
            public bool IsProductInvoice { get; set; }
            public int PriceLevelId { get; set; }
            public int VersionReleaseId { get; set; }
            public string InvoiceTitle { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal Price { get; set; }
            public string Notes { get; set; }
            public int Discriminator { get; set; } // ForEver = 1,Monthly =2 ,Yearly = 3
            public bool IsAdminInvoice { get; set; }
            public int? VersionSubscriptionId { get; set; }
            public int AdminInvoiceTypeId { get; set; }

        }
        public class UpdateAdminInvoiceInputDto
        {
            public int VersionId { get; set; }
            public int? AddOnId { get; set; }
            public bool IsProductInvoice { get; set; }
            public bool IsInvoiceSupportAdmin { get; set; }
            public int PriceLevelId { get; set; }
            public int VersionReleaseId { get; set; }
            public string InvoiceTitle { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal Price { get; set; }
            public string Notes { get; set; }
            public int Discriminator { get; set; } // ForEver = 1,Monthly =2 ,Yearly = 3
            public int InvoiceId { get; set; }
            public bool IsAdminInvoice { get; set; }
            public int? VersionSubscriptionId { get; set; }
        }
        public class GetCustomerVersionOrAddOnOutputDto
        {
            public IEnumerable<GetVersionsubscriptionInvoiceOutputDto> Version { get; set; }
            public IEnumerable<GetAddOnSubscriptionOutputDto> AddOn { get; set; }

        }
        public class GetCustomerInvoiceOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Account { get; set; }
            public string TaxReg { get; set; }
            public string Address { get; set; }
        }
        public class GetVersionsubscriptionInvoiceOutputDto
        {
            public int VersionSubscriptionId { get; set; }
            public int VersionId { get; set; }
            public int VersionReleaseId { get; set; }
            public int CustomerSubscriptionId { get; set; }
            public string Name { get; set; }
            public string ApplicationName { get; set; }
            public decimal Price { get; set; }
            public decimal SupportPrice { get; set; }
            public decimal VatPrice { get; set; }
            public decimal NetPrice { get; set; }
            public decimal Total { get; set; }
            public FileStorageDto Logo { get; set; }
            public int LicensesCount { get; set; }

            //Todo
            public RetrievePriceLevelDto PriceLevel { get; set; }
            public IEnumerable< RetrievePriceLevelDto> PriceLevels { get; set; }
            public DateTime RenewDate { get; set; }
            public string Currency { get; set; }
            public GetSubscriptionTypeOutputDto Subscription { get; set; }
            public GetInvoicePriceOutputDto ProductPrice { get; set; }

        }
       
        public class GetAddOnSubscriptionOutputDto
        {
            public int AddOnsubscriptionId { get; set; }
            public int AddOnId { get; set; }
            public int VersionId { get; set; }
            public int VersionReleaseId { get; set; }
            public int VersionSubscriptionId { get; set; }
            public int CustomerSubscriptionId { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public decimal SupportPrice { get; set; }
            public decimal VatPrice { get; set; }
            public decimal NetPrice { get; set; }
            public decimal Total { get; set; }
            //Todo replace this data
            public FileStorageDto Logo { get; set; }
            //Todo
            public DateTime RenewDate { get; set; }
            public RetrievePriceLevelDto PriceLevel { get; set; }
            public string Currency { get; set; }
            public GetSubscriptionTypeOutputDto Subscription { get; set; }
            public GetInvoicePriceOutputDto ProductPrice { get; set; }

        }
        public class GetInvoicePriceOutputDto
        {
            public decimal VatPercentage { get; set; }

            public decimal TotalVatAmount { get; set; }

            public decimal Discount { get; set; }

            public decimal SubTotal { get; set; }

            public decimal Total { get; set; }
        }
        public class GetInvoicesOutputDto
        {
            public int Id { get; set; }
            public string ContractId { get; set; }
            public string Serial { get; set; }
            public string Customer { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime CreateDate { get; set; }
            public string Version { get; set; }
            public string Notes { get; set; }
            public string PaymentMethd { get; set; }
            public string InvoiceStatus { get; set; }
            public string Currency { get; set; }
            public decimal SubTotal { get; set; }
            public decimal TotalDiscountAmount { get; set; }
            public decimal TotalVatAmount { get; set; }
            public decimal VatPercentage { get; set; }
            public decimal Total { get; set; }
        }
        public class GetCustomerDefaultTaxInputDto
        {
            public int CustomerId { get; set; }

            [JsonIgnore]
            public int CountryId { get; set; }
            [JsonIgnore]
            public GetCountryDefaultTaxInputDto Tax { get; set; }

         
        }
        public class GetAddOnInputDto
        {
            public int VersionSubscriptionId { get; set; }
        }
        public class GetCustomerDefaultTaxApiInputDto
        {
            public int CustomerId { get; set; }
        }

        public class GetAddOnCanPurshasedOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public AddonPriceDetailsDto Price { get; set; }
            public FileStorageDto Logo { get; set; }
            public IEnumerable<RetrievePriceLevelDto> Pricelevel { get; set; }
        }
        public class GetVersionCanPurshasedOutputDto
        {
            public int Id { get; set; }
            public int ApplicationId { get; set; }
            public string Name { get; set; }
            public int VersionReleaseId { get; set; }
            public string ApplicationName { get; set; }
            public int SubscriptionTypeId { get; set; }
            public VersionPriceDetailsDto Price { get; set; }
            public List<RetrieveVersionPrice> Prices { get; set; }
            public FileStorageDto Logo { get; set; }
            public IEnumerable< RetrievePriceLevelDto> PriceLevels { get; set; }
        }
        public class RetrieveVersionPrice 
        {
            public decimal MonthlyPrice { get; set; }
            public decimal YearlyPrice { get; set; }
            public decimal ForEverPrice { get; set; }
            public RetrievePriceLevelDto PriceLevel { get; set; }
        }
    }
}
