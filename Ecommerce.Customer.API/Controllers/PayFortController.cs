using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Ecommerce.BLL.Customers.Invoices;
using Ecommerce.BLL.Payments;
using Ecommerce.DTO.Settings.EmailTemplates;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers
{
	[Route("[controller]")]
    [ApiController]
	public class PayFortController : Controller
    {
        private readonly IPaymentBLL _paymentBLL;
        public readonly EmailTemplateSetting _emailTemplateSetting;
        public readonly IWebHostEnvironment _env; public PayFortController( IPaymentBLL paymentBLL, IOptions<EmailTemplateSetting> emailTemplateSetting )
        {
            _paymentBLL = paymentBLL;
            _emailTemplateSetting = emailTemplateSetting.Value;
        }

            [HttpGet("Index")]
        public IActionResult Index( )
        {

            return View();
        }
        [HttpPost("RedirectPost")]
        public async Task<IActionResult> RedirectPost( )
        {
            if (Request.Form != null)
            {
                var output = await _paymentBLL.CallBackConfirmBankOrder(Request?.Form);
                return RedirectPermanent(_emailTemplateSetting.CLientBaseUrl + "/fawry-card?status=1");

            }
            else
                 return RedirectPermanent(_emailTemplateSetting.CLientBaseUrl+"/fawry-card?status=0");

        }
        [HttpGet("RedirectGet")]
        public async Task<IActionResult> RedirectGet( )
        {
            if (Request.Form != null)
            {
                var output = await _paymentBLL.CallBackConfirmBankOrder(Request?.Form);
                return RedirectPermanent(_emailTemplateSetting.CLientBaseUrl + "/fawry-card?status=1");

            }
            else
                return RedirectPermanent(_emailTemplateSetting.CLientBaseUrl + "/fawry-card?status=0");
        }
    }
}
