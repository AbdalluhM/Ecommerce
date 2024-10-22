using Dexef.Payment.FawryAPI;
using Dexef.Payment.Models.PAYFORT;

using Microsoft.AspNetCore.Http;

using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.DTO.Customers.Cards;
using Ecommerce.DTO.Customers.Invoices;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;

namespace Ecommerce.BLL.Payments
{
    public interface IPaymentBLL
    {
        #region CRUD

        Task<IResponse<bool>> CreateAsync(CardTokenDto inputDto);
        Task<IResponse<bool>> UpdateAsync(UpdateCardTokenDto inputDto);
        Task<IResponse<bool>> DeleteAsync(int customerId, string cardToken, int paymentMethodId, bool commit = false);
        Task<IResponse<List<CardTokenDto>>> GetAllAsync();
        #endregion
        Task<IResponse<string>> CreateCardToken(CreateCardTokenInputDto inputDto);
        Task<IResponse<bool>> CreateCardTokenCallBack(CreateCardTokenCallBackDto inputDto);
        Task<IResponse<bool>> DeleteCardToken(int customerId, string cardToken, int paymentMethodId);
        Task<IResponse<bool>> UpdateCardToken(UpdateCardTokenDto inputDto);
        Task<IResponse<List<CardTokenDto>>> ListCustomerCards(int customerId, int paymentMethodId);

        Task<IResponse<string>> FawryCreateCardToken(FawryAPICreateTokenInputDto inputDto);
        Task<IResponse<bool>> FawryDeleteCardToken(int CustomerId, string cardToken);
        Task FawryDeleteAllCardToken();
        Task FawryDeleteAllCustomerCardToken(int customerId);
        Task<IResponse<ListCustomerTokensResponse>> FawryListCustomerCards(int CustomerId);
        Task<InvoicePaymentInfoJson> Pay(ThirdPartyInvoiceInputDto paymentData, InvoiceDto invoice);
        Task<GetPaymentMethodsOutputDto> GetCustomerDefaultPaymentMethodAsnyc(int customerId);
        Task<InvoicePaymentInfoJson> PayInvoiceAsync(PaymentDto payDto);
        Task<InvoicePaymentInfoJson> PayWithPayPal(ThirdPartyInvoiceInputDto paymentData, InvoiceDto invoice);
        Task<bool> UpdatePaypalCaptureId(int referenceId, string captureId);
        Task<bool> UpdateBankFortIdAndCardData(int referenceId, string fortId, string paymentOption, string cardNumber);
        Task<bool> UpdateFawryCardReferenceCodeAndCardData(int referenceId, string fawryReferenceId, string cardNumber);
        Task<bool> CallBackConfirmPayPalOrder(string token, string payerId);
        Task<RefundPaymentResponseDto> RefundPayPal(Invoice invoice);
        Task<InvoicePaymentInfoJson> PayWithFawry(ThirdPartyInvoiceInputDto paymentData, InvoiceDto invoice);
        Task DeleteCustomerTokens(int customerId);
        Task<IResponse<bool>> FawryVerifyNotificationCallBack(string requestCallBack);
        Task<PaymentStatusResponse> FawryGetPaymentStatus(string merchantRefNumber);
        Task<PaymentStatusResponse> FawryTestGetPaymentStatus(string merchantRefNumber);
        Task<IResponse<bool>> CallBackConfirmFawryOrder(string type, string referenceNumber, string merchantRefNumber, string orderAmount, string fawryFees, string orderStatus, string paymentMethod, string paymentTime, string customerMobile, string customerMail, string customerProfileId, string signature, string taxes, int statusCode, string statusDescription, bool basketPayment = false);
        Task<IResponse<bool>> CallBackConfirmPayMobOrder(int merchantOrderNumber, bool success);
        Task<int> UpdateFawryInvoiceAsync(string merchantRefNumber, string orderStatus, string paymentResponse, bool commit = true);
        Task<RefundPaymentResponseDto> RefundFawry(Invoice invoice);
        Task<InvoicePaymentInfoJson> PayWithBank(ThirdPartyInvoiceInputDto paymentData, InvoiceDto invoice);
        Task<IResponse<PayFortPurchaseResponse>> CallBackConfirmBankOrder(IFormCollection formParameters);
        Task<RefundPaymentResponseDto> RefundBank(Invoice invoice);
        IResponse<bool> UpdateCardTypeAsync(UpdateCardTypeInputDto inputDto);
        Task SyncCardTokens(CreateCardTokenInputDto inputDto);
        Task<IResponse<string>> TryToPay(APIPayAndSubscribeInputDto inputDto, CustomerSubscription customerSubscription, Invoice invoice = null, bool commit = false);
        Task<int> GetPaymentMethodId(int customerId);
        APIPayAndSubscribeInputDto MapedPaymentData(PaymentDetailsInputDto inputDto, APIPayAndSubscribeInputDto paymentDetails);
    }
}
