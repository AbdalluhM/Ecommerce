using Dexef.Payment.FawryAPI;

using Hangfire;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Ecommerce.BLL.Contracts.Invoices;
using Ecommerce.BLL.Customers.Invoices;
using Ecommerce.BLL.Customers.Invoices.Job;
using Ecommerce.BLL.Payments;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Json;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.EmailTemplates;
using Ecommerce.Helper.String;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.Customer.API.Controllers.Invoices
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : BaseCustomerController
    {
        private readonly IInvoiceBLL _invoiceBLL;
        private readonly IPaymentSetupBLL _paymentSetupBLL;
        public readonly EmailTemplateSetting _emailTemplateSetting;
        public readonly IWebHostEnvironment _env;
        private readonly IInvoiceJobBLL _invoiceJobBLL;
        private readonly IAdminInvoiceBLL _adminInvoiceBLL;
        private readonly IPaymentBLL _paymentBLL;

        public InvoiceController(IInvoiceBLL invoiceBLL,
            IPaymentSetupBLL paymentSetupBLL,
            IOptions<EmailTemplateSetting> emailTemplateSetting,
            IInvoiceJobBLL invoiceJobBLL, IWebHostEnvironment env,
            IAdminInvoiceBLL adminInvoiceBLL,
                              IHttpContextAccessor httpContextAccessor, IPaymentBLL paymentBLL) : base(httpContextAccessor)
        {
            _invoiceBLL = invoiceBLL;
            _paymentSetupBLL = paymentSetupBLL;
            _emailTemplateSetting = emailTemplateSetting.Value;
            _invoiceJobBLL = invoiceJobBLL;
            _env = env;
            _adminInvoiceBLL = adminInvoiceBLL;
            _paymentBLL = paymentBLL;
        }

        [Authorize]
        [HttpGet("GetPagedList")]
        [Produces(typeof(IResponse<PagedResultDto<RetrieveInvoiceDto>>))]
        public IActionResult GetInvoices([FromQuery] FilterInvoiceDto filterInvoiceDto)
        {

            var result = _invoiceBLL.GetInvoices(filterInvoiceDto, CurrentUserId);

            return Ok(result);
        }
        [Authorize]
        [HttpGet("{id}")]
        [Produces(typeof(IResponse<RetrieveInvoiceDto>))]
        public async Task<IActionResult> Get(int id)
        {
            var output = await _invoiceBLL.GetInvoiceById(new GetInvoiceInputDto { Id = id });

            return Ok(output);

        }
        [Authorize]
        [HttpGet("GetPaymentMethods")]
        public async Task<IActionResult> GetPaymentMethods()
        {

            var output = await _paymentSetupBLL.GetAllPaymentMethodsAsync(CurrentUserCountryId);
            return Ok(output);
        }

        [Authorize]
        [HttpGet("GetPaymentMethodByPaymentType")]
        public async Task<IActionResult> GetPaymentMethodByPaymentType(PaymentTypesEnum paymentTypeId)
        {
            var output = await _paymentSetupBLL.GetPaymentMethodByPaymentTypeWithResponse(paymentTypeId);

            return Ok(output);
        }

        [Authorize]
        [HttpPost("GetInvoicePaymentDetails")]
        public async Task<IActionResult> GetInvoicePaymentDetails(int invoiceId)
        {
            var input = new InvoicePaymentDetailsInputDto
            {
                InvoiceId = invoiceId,
                CountryId = CurrentUserCountryId,
            };
            var output = await _invoiceBLL.GetInvoicePaymentDetails(input);
            return Ok(output);

        }
        [Authorize]
        [HttpPost("GetApplicationPaymentDetails")]
        public async Task<IActionResult> GetApplicationPaymentDetails([FromQuery] APIPaymentDetailsInputDto inputDto)
        {
            var input = new PaymentDetailsInputDto
            {
                VersionId = inputDto.VersionId,
                AddOnId = inputDto.AddOnId,
                PriceLevelId = inputDto.PriceLevelId,
                Discriminator = inputDto.Discriminator,
                VersionSubscriptionId = inputDto.VersionSubscriptionId,
                //TODO:Check by m said
                CountryId = inputDto.PaymentCountryId,
                CustomerId = CurrentUserId
            };
            var output = await _invoiceBLL.GetVersionPaymentDetails(input);
            return Ok(output);

        }

        [HttpPost("PayAndSubscribe")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        [Produces(typeof(IResponse<GetPayAndSubscribeOutputDto>))]
        public async Task<IActionResult> PayAndSubscribe(APIPayAndSubscribeInputDto inputDto)
        {
            var paymentTypeResult = new Response<GetPayAndSubscribeOutputDto>();

            //string baseAPIUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("~"));
            string returnUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("~")).JoinUriSegments("/api/Invoice/Bank/ConfirmOrderCallBack");
            if (_env.IsStaging() || _env.IsProduction()) //  azure
                returnUrl = string.Empty;

            var paymentType = await _paymentSetupBLL.GetPaymentTypeByPaymentMethod(inputDto.PaymentMethodId);
            if (paymentType == null)
                return Ok(paymentTypeResult.CreateResponse(MessageCodes.InvalidPaymentMethod));
            string url = paymentType.Id switch
            {

                //(int)PaymentTypesEnum.Fawry => inputDto.IsFawryCard ? baseAPIUrl + "/api/Invoice/Fawry/CallBackThreeDSecureUrl" : inputDto.SuccessCallBackUrl,
                (int)PaymentTypesEnum.Bank => returnUrl,
                _ => inputDto.SuccessCallBackUrl
            };

            inputDto.SuccessCallBackUrl = url;
            inputDto.CustomerId = CurrentUserId;
            var output = await _invoiceBLL.PayAndSubscribe(inputDto);
            return Ok(output);

        }
        #region Payments
        #region PayPal
        [HttpGet("CapturePayPalOrder")]
        public async Task<IActionResult> CallBackConfirmPayPalOrder(string token, string payerId)
        {

            var output = await _paymentBLL.CallBackConfirmPayPalOrder(token, payerId);
            return Ok(output);

        }
        #endregion
        #region Fawry
        [HttpPost("Fawry/CreateCardToken")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> FawryCreateCardToken([FromBody] FawryAPICreateTokenInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = await _paymentBLL.FawryCreateCardToken(inputDto);
            return Ok(output);
        }

        [HttpPost("Fawry/DeleteCardToken")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> DeleteCardToken(string cardToken)
        {

            var output = await _paymentBLL.FawryDeleteCardToken(CurrentUserId, cardToken);
            return Ok(output);

        }

        [HttpGet("Fawry/ListCardTokens")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> ListCardTokens()
        {

            var output = await _paymentBLL.FawryListCustomerCards(CurrentUserId);
            return Ok(output);

        }


        [HttpGet("Fawry/CallBackThreeDSecureUrl")]
        public async Task<IActionResult> CallBackConfirmFawryOrder()
        {
            var result = new Response<bool>() as IResponse<bool>;

            var queryString = Request.QueryString.Value;
            var parameters = queryString.GetQueryParameters();
            try
            {

                result = await _paymentBLL.CallBackConfirmFawryOrder(
                     parameters["type"],
                     parameters["referenceNumber"],
                     parameters["merchantRefNumber"],
                     parameters["orderAmount"],
                     parameters["fawryFees"],
                     parameters["orderStatus"],
                     parameters["paymentMethod"],
                     parameters["paymentTime"],
                     parameters["customerMobile"],
                     parameters["customerMail"],
                     parameters["customerProfileId"],
                     parameters["signature"],
                     parameters["taxes"],
                     Convert.ToInt32(parameters["statusCode"]),
                     parameters["statusDescription"],
                     Convert.ToBoolean(parameters["basketPayment"]));
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => _invoiceJobBLL.UpdateFawryReferenceInvoices());
            }

            return Ok(result);
        }
        [HttpGet("PayMob/CallBackThreeDSecureUrl")]
        public async Task<IActionResult> CallBackConfirmPayMobOrder(string merchant_order_id, bool success)
        {

            var merchantOrderNumber = int.Parse(merchant_order_id.Split("##").Last());
            var result = await _paymentBLL.CallBackConfirmPayMobOrder(merchantOrderNumber, success);

            if (!(result.IsSuccess && result.Data))
                return Redirect("https://my.dexef.com/paymentfailed");

            return Redirect("https://my.dexef.com/paymentsuccesspaymob");


        }

        [HttpPost("Fawry/VerifyNotificaion")]
        public async Task<IActionResult> VerifyNotificaion([FromBody] FawryV2NotificationResponse request)
        {
            try
            {
                var requestBodyString = request?.Serialize();
                //BackgroundJob.Schedule(
                //( ) => _invoiceBLL.FawryVerifyNotificationCallBack(requestBodyString),
                //TimeSpan.FromMinutes(1));
                BackgroundJob.Enqueue(() =>
               _paymentBLL.FawryVerifyNotificationCallBack(requestBodyString));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Serialize Data");

            }

            return Ok();
        }

        [HttpGet("Fawry/GetPaymentStatus")]
        public async Task<IActionResult> GetPaymentStatus(string merchantRefNumber)
        {
            var x = _paymentBLL.FawryGetPaymentStatus(merchantRefNumber);
            return Ok(x);
        }

        #endregion
        #region Bank
        //Returns
        //{"isSuccess":true,"errors":[],"data":{"customerEmail":"ahmed.saed@dexef.net","signature":"aa2c34fd66500b5add8d99b1346db06e720f5d1c44ec833affa5173a66e00ca0","paymentOption":"VISA","eci":"ECOMMERCE","orderDescription":"Invoice Serial : INV_SER","customerIp":"156.215.201.183","customerName":null,"cardHolderName":"John Smith","expiryDate":"2505","cardNumber":"400555******0001","command":"PURCHASE","merchantIdentifier":"3dc28a2f","merchantReference":"1782","amount":"7150","currency":"EGP","language":"en","fortId":"169996200006467177","responseMessage":"Success","responseCode":"14000","status":"14","isSuccess":false}}
        [HttpPost("Bank/ConfirmOrderCallBack")]
        public async Task CallBackConfirmBankOrder()
        {
            var output = await _paymentBLL.CallBackConfirmBankOrder(Request.Form);
            //string callbackUrl = _emailTemplateSetting.CLientBaseUrl.JoinUriSegments("fawry-card?status={0}&type={1}");
            // int type = 0;//TODO: Update this // 0 for application and 1 for addon
            string callbackUrl = _emailTemplateSetting.CLientBaseUrl.JoinUriSegments("fawry-card?status={0}");
            bool status = output.IsSuccess && output.Data.IsSuccess;
            callbackUrl = String.Format(callbackUrl, status ? 1 : 0);
            Response.Redirect(callbackUrl);
        }


        #endregion
        #endregion

        [HttpPost("CreateUnPaidInvoice")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> CreateUnPaidInvoice(PaymentDetailsInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            inputDto.CountryId = CurrentUserCountryId;
            inputDto.IsFawryCard = true;
            inputDto.IsUnPaid = true;
            inputDto.InvoiceStatusId = (int)InvoiceStatusEnum.WaitingPaymentConfirmation;
            var output = await _invoiceBLL.CreateUnPaidInvoice(inputDto);
            return Ok(output);
        }





        [HttpGet("GetTicketInvoicesLookup")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> GetTicketInvoicesLookupAsync()
        {
            var result = await _invoiceBLL.GetTicketInvoicesLookupAsync(CurrentUserId);

            return Ok(result);
        }

        [HttpPost("GenerateInvoiceQrCode")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> GenerateInvoiceQrCodeAsync(int invoiceId)
        {
            var result = await _invoiceBLL.GenerateInvoiceQrCodeAsync(invoiceId);

            return Ok(result);
        }

        [HttpPost("DownloadInvoices")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<IActionResult> DownloadInvoicesAsync(IEnumerable<int> invoicesIds)
        {
            var result = await _adminInvoiceBLL.DownloadInvoicesAsync(invoicesIds, CurrentEmployeeLanguage);
            return Ok(result);
        }
    }
}
