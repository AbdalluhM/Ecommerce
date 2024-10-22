using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Json;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.DTO.Customers.Invoices;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Taxes;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;

namespace Ecommerce.BLL.Customers.Invoices
{
    public interface IInvoiceHelperBLL
    {
        public InvoiceTotals CalculateInvoice( decimal itemPrice, decimal discount, Tax tax, bool firstPurchase, string title );
        public InvoiceTotals CalculateInvoice( VersionPrice vp, Tax tax, int discriminator, bool firstPurchase );
        public InvoiceTotals CalculateInvoice( AddOnPrice ap, Tax tax, int discriminator, bool firstPurchase );
        List<CreateInvoiceDetailInputDto> CalculateInvoiceDetailsFromPreviousInvoice( Invoice previousInvoice, Tax tax );
        Invoice CalculateInvoiceFromPreviousInvoice( Invoice previousInvoice );
        int CalculateNumberOfInvoicesToCreate( DateTime lastInvoiceDate, int renewEvery );
        AddonSubscription CreateOrGetAddOnSubscription( APIPayAndSubscribeInputDto inputDto, CustomerSubscription customerSubscription, VersionSubscription versionSubscription, bool commit = false );
        Task<Contract> CreateOrGetCustomerContractAsync( int customerId, bool commit = false );
        CustomerSubscription CreateOrGetCustomerSubscription( APIPayAndSubscribeInputDto inputDto, bool commit = false );
        Task<Invoice> CreateOrGetInvoiceAsync( APIPayAndSubscribeInputDto inputDto, Contract contract, CustomerSubscription customerSubscription, VersionSubscription versionSubscription, AddonSubscription addonSubscription, bool commit = false );
        IResponse<VersionSubscription> CreateOrGetVersionSubscription( APIPayAndSubscribeInputDto inputDto, CustomerSubscription customerSubscription, bool commit = false );
        string CreatePaymentResultForSearch( int PaymentTypeId, InvoicePaymentInfoJson paymentInfo );
        Task<string> GenerateInvoiceEmailTemplate( Invoice invoice );
        string GenerateInvoiceSerial( int invoiceId = 0, string contractSerial = null );
        Task<Invoice> GetInvoiceById( int invoiceId );
        Task<RetrieveInvoiceDto> GetMappedInvoiceById( int invoiceId );
        string GetConcatenatedJsonString(string separator, params string [] jsonNames );
        string GetCurrencyCode( string objCode, LangEnum lang = LangEnum.Default );
        int GetInvoiceDiscriminator( Invoice invoice );
        int GetInvoiceDiscriminator( RetrieveInvoiceDto invoice );
        int GetInvoiceDiscriminator( int invoiceTypeId, int renewEvery );
        List<InvoicePeriodDto> GetInvoicePeriods( DateTime lastInvoiceDate, int renewEvery );
        GetSupportInvoicePriceOutputDto GetInvoicePrice( decimal Price, GetTaxOutputDto tax );
        int GetInvoiceRenewEvery( int discriminator );
        InvoiceTotals GetInvoiceSummary( Invoice invoice );
        int GetInvoiceType( int discriminator, bool firstSubscription );
        int GetInvoiceTypeFromPreviousInvoice( int previousInvoiceTypeId );
        DateTime GetRenewalDate( int customerSubscriptionId );
        int GetSubscriptionCurrency( VersionSubscription versionSubscription, AddonSubscription addonSubscription = null, bool isAddOn = false );
        int GetSubscriptionId( VersionSubscription versionSubscription, AddonSubscription addonSubscription = null, bool isAddOn = false );
        bool IsFirstCustomerSubscription( int customerId, int id, int priceLevelId, bool isAddon );
        bool IsFirstCustomerSubscription( int versionSubscriptionId, int? addOnSubscriptionId, bool isAddOn, bool isProductService );
        bool IsFirstCustomerSubscriptionByInvoice( int invoiceId );
        bool IsFirstCustomerSubscriptionBySubscription( int customerSubscriptionId );
        bool IsSubscriptionHasAnyPaidInvoice( int versionSupscriptionId );
        Task<Contract> QualifyCustomerAsync( int customerId, bool isFirstSubscription, bool commit );
        bool RollBackSubscription( int customerSubscriptionId );
        bool RollBackSubscriptionByInvoiceId( int invoiceId );
        Task SendInvoicesEmails( List<Invoice> invoices, bool hasAttachment = false );
        bool UpdateInvoicePaymentInfoForSearch( Invoice invoice, InvoicePaymentInfoJson paymentInfo, bool commit = false );
        Task<bool> UpdateInvoiceSerialAsync( Invoice invoice, Contract contract = null, bool commit = false );
        Task<bool> UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync( int invoiceId, PaymentTypesEnum paymentTypeId, int status = (int)InvoiceStatusEnum.Paid, string paymentResponse = null, bool commit = true );

        Task<InvoiceDto> UpdateInvoiceTotalsWithConvertedCurrencyAsync( InvoiceDto invoice, string fromCurrency, string toCurrency );
    }
}
