using Dexef.Payment.FawryAPI;
using Dexef.Payment.Models.PAYFORT;

using Microsoft.AspNetCore.Http;

using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Json;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.DTO.Customers.Invoices;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;

namespace Ecommerce.BLL.Customers.Invoices
{
    public interface IInvoiceBLL
    {
        IResponse<PagedResultDto<RetrieveInvoiceDto>> GetInvoices(FilterInvoiceDto pagedDto, int currentUserId);
        Task<IResponse<RetrieveInvoiceDto>> GetInvoiceById( GetInvoiceInputDto inputDto );
        Task<IResponse<APIPayAndSubscribeInputDto>> GetVersionPaymentDetails( PaymentDetailsInputDto inputDto );
        Task<IResponse<APIPayAndSubscribeInputDto>> GetInvoicePaymentDetails( InvoicePaymentDetailsInputDto inputDto );
        Task<IResponse<GetPayAndSubscribeOutputDto>> PayAndSubscribe( APIPayAndSubscribeInputDto inputDto );
    
        Task<RefundPaymentResponseDto> RefundInvoiceAsync(Invoice invoice);
        //Task<GetPaymentMethodsOutputDto> GetCustomerDefaultPaymentMethodAsnyc( int customerId );
        Task<Invoice> CreateInvoiceAsync( NewInvoiceDto teoldInvoice );
        //Task<InvoicePaymentInfoJson> PayInvoiceAsync( PaymentDto payDto );
        //#region PayPal
        //Task<bool> CallBackConfirmPayPalOrder( string token, string payerId );



        //#endregion
        //#region Fawry
        //Task<IResponse<string>> FawryCreateCardToken( FawryAPICreateTokenInputDto inputDto );
        //Task<IResponse<CardTokenResponse>> FawryDeleteCardToken( int CustomerId, string cardToken );
        //Task<IResponse<ListCustomerTokensResponse>> FawryListCustomerCards( int CustomerId );
        //Task<IResponse<bool>> CallBackConfirmFawryOrder( string type, string referenceNumber, string merchantRefNumber, string orderAmount, string fawryFees, string orderStatus, string paymentMethod, string paymentTime, string customerMobile, string customerMail, string customerProfileId, string signature, string taxes,
        //    int statusCode, string statusDescription, bool basketPayment = false );
        //Task<IResponse<bool>> FawryVerifyNotificationCallBack( string requestCallBack );
        //Task<PaymentStatusResponse> FawryGetPaymentStatus( string merchantRefNumber );
        Task<IResponse<int>> CreateUnPaidInvoice( PaymentDetailsInputDto inputDto );

        //#endregion
        //#region Bank
        //Task<IResponse<PayFortPurchaseResponse>> CallBackConfirmBankOrder( IFormCollection formParameters );
        //#endregion

        Task<IResponse<IEnumerable<TicketInvoiceLookupDto>>> GetTicketInvoicesLookupAsync(int customerId);

        Task<IResponse<string>> GenerateInvoiceQrCodeAsync(int invoiceId);

        bool IsVersionHasNotRefundedAddons(VersionSubscription versionSub);
  
    }
}
