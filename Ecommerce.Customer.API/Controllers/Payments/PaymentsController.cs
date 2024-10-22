using Hangfire;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Payments;
using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.Cards;
using Ecommerce.DTO.Settings.EmailTemplates;
using Ecommerce.Helper.String;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers
{
    // [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : BaseCustomerController
    {
        public readonly IPaymentBLL _paymentBLL;

        public readonly EmailTemplateSetting _enviromentSetting;


        public PaymentsController(IPaymentBLL paymentBLL, IOptions<EmailTemplateSetting> enviromentSetting,
                              IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _paymentBLL = paymentBLL;
            _enviromentSetting = enviromentSetting.Value;

        }
        #region All
        [Authorize]
        [HttpPost("CreateCardToken")]
        public async Task<IActionResult> CreateCardToken([FromBody] CreateCardTokenInputDto inputDto)
        {
            inputDto.CustomerId = CurrentUserId;
            var output = await _paymentBLL.CreateCardToken(inputDto);
            return Ok(output);
        }

        [HttpPost("CreateCardTokenCallBack")]
        public async Task<IActionResult> CreateCardTokenCallBack([FromBody] CreateCardTokenCallBackDto inputDto)
        {
            var output = await _paymentBLL.CreateCardTokenCallBack(inputDto);
            return Ok(output);
        }
        [HttpGet("TestCreateCardTokenCallBack")]
        public async Task<IActionResult> TestCreateCardTokenCallBack()
        {
            var queryString = Request.QueryString.Value;
            var parameters = queryString.GetQueryParameters();
            var input = new CreateCardTokenCallBackDto();
            var output = await _paymentBLL.CreateCardTokenCallBack(input);
            return Ok(output);
        }
        [Authorize]
        [HttpPost("UpdateCardToken")]
        public async Task<IActionResult> UpdateCardToken([FromBody] UpdateCardTokenDto inputDto)
        {
            var output = await _paymentBLL.UpdateCardToken(inputDto);
            return Ok(output);

        }
        [Authorize]
        [HttpPost("DeleteCardToken")]
        public async Task<IActionResult> DeleteCardToken(string cardToken, int paymentMethodId)
        {
            var output = await _paymentBLL.DeleteCardToken(CurrentUserId, cardToken, paymentMethodId);
            return Ok(output);

        }
        [Authorize]
        [HttpGet("ListCardTokens")]
        [Produces(typeof(IResponse<List<CardTokenDto>>))]
        public async Task<IActionResult> ListCardTokens(int paymentMethodId)
        {

            var output = await _paymentBLL.ListCustomerCards(CurrentUserId, paymentMethodId);
            return Ok(output);

        }
        [HttpGet("Fawry/ListCardTokens")]
        public async Task<IActionResult> FawryListCardTokens(int customerId)
        {
            var output = await _paymentBLL.FawryListCustomerCards(customerId);
            return Ok(output);

        }
        [HttpGet("Fawry/DeleteCardTokens")]
        public async Task<IActionResult> FawryDeleteCardTokens(int customerId, string cardToken)
        {

            var output = await _paymentBLL.FawryDeleteCardToken(customerId, cardToken);
            return Ok(output);

        }
        [HttpGet("Fawry/DeleteAllCardTokens")]
        public async Task<IActionResult> DeleteAllCardTokens()
        {

            BackgroundJob.Enqueue(() => _paymentBLL.FawryDeleteAllCardToken());
            return Ok();

        }
        [HttpGet("Fawry/DeleteCustomerAllCardTokens")]
        public async Task<IActionResult> DeleteCustomerAllCardTokens(int customerId)
        {

            BackgroundJob.Enqueue(() => _paymentBLL.FawryDeleteAllCustomerCardToken(customerId));
            return Ok();

        }

        #endregion

        //#region Fawry
        //[Authorize]
        //[HttpPost("Fawry/CreateCardToken")]
        //public async Task<IActionResult> FawryCreateCardToken( [FromBody] FawryAPICreateTokenInputDto inputDto )
        //{
        //    inputDto.CustomerId = CurrentUserId;
        //    var output = await _paymentBLL.FawryCreateCardToken(inputDto);
        //    return Ok(output);
        //}

        //[Authorize]
        //[HttpPost("Fawry/DeleteCardToken")]
        //public async Task<IActionResult> DeleteCardToken( string cardToken)
        //{

        //    var output = await _paymentBLL.FawryDeleteCardToken(CurrentUserId, cardToken);
        //    return Ok(output);

        //}
        //[Authorize]
        //[HttpGet("Fawry/ListCardTokens")]
        //public async Task<IActionResult> ListCardTokens( )
        //{

        //    var output = await _paymentBLL.FawryListCustomerCards(CurrentUserId);
        //    return Ok(output);

        //}

        ////type=ChargeResponse&referenceNumber=7101771798&merchantRefNumber=294&orderAmount=55&paymentAmount=55&fawryFees=0&orderStatus=PAID&paymentMethod=PayUsingCC&paymentTime=1656925277466&customerMobile=01012017784&customerMail=swilam%40mailsac.com&customerProfileId=1185&signature=4e824b9dd7b7de2128503757bcbcbca4786a2d81501ae167f46fbf87686139fc&taxes=0&statusCode=200&statusDescription=Operation%20done%20successfully&basketPayment=false
        //[HttpGet("Fawry/CallBackThreeDSecureUrl")]
        //public async Task<IActionResult> CallBackThreeDSecureUrl( string type, string referenceNumber, string merchantRefNumber, string orderAmount, string fawryFees, string orderStatus, string paymentMethod, string paymentTime, string customerMobile, string customerMail, string customerProfileId, string signature, string taxes,
        //    int statusCode, string statusDescription, bool basketPayment = false )
        //{

        //    await _paymentBLL.CallBackThreeDSecureUrl(type, referenceNumber, merchantRefNumber, orderAmount, fawryFees, orderStatus, paymentMethod, paymentTime, customerMobile, customerMail, customerProfileId, signature, taxes,
        //     statusCode, statusDescription, basketPayment);
        //    return Ok();

        //}

        ////[HttpGet("Fawry/VerifyNotificaion/{invoiceId}")]
        ////public async Task<IActionResult> VerifyNotificaion( string invoiceId )
        ////{
        ////    await _invoiceBLL.CallBackThreeDSecureUrl(invoiceId);
        ////    return Ok();
        ////}

        //#endregion

    }
}

