using Dexef.Notification.Enums;
using Dexef.Notification.Models.Mailing;
using Dexef.Notification.Models.Sms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.ContactUs;
using Ecommerce.BLL.Customers.Crm;
using Ecommerce.BLL.LookUps;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Pdfs;
using Ecommerce.Core.Enums.Notifications.Sms;
using Ecommerce.DTO.ContactUs;
using Ecommerce.DTO.Pdfs.Invoices;
using Ecommerce.DTO.Settings.Auth;
using Ecommerce.DTO.Settings.Pdfs.PdfSettings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers.ContactUs
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsBLL _contactUsBLL;
        private readonly ILookUpBLL _lookUpBLL;
        private readonly IPdfGeneratorBLL _pdfGeneratorBLL;
        private readonly InvoicePdfSetting _invoicePdfSetting;
        private readonly INotificationBLL _notificationBLL;
        private readonly ICrmBLL _crmBll;
        private readonly AuthSetting _authSetting;

        public ContactUsController(IContactUsBLL contactUsBLL,
                                   ILookUpBLL lookUpBLL,
                                   IPdfGeneratorBLL pdfGeneratorBLL,
                                   IOptions<InvoicePdfSetting> invoicePdfSetting,
                                   INotificationBLL notificationBLL,
                                   ICrmBLL crmBll,
                                   IOptions<AuthSetting> authSetting)
        {
            _contactUsBLL = contactUsBLL;
            _lookUpBLL = lookUpBLL;
            _pdfGeneratorBLL = pdfGeneratorBLL;
            _invoicePdfSetting = invoicePdfSetting.Value;
            _notificationBLL = notificationBLL;
            _crmBll = crmBll;
            _authSetting = authSetting.Value;
        }

        [HttpGet("Country/GetAll")]
        public IActionResult GetAllCountries()
        {
            var response = _lookUpBLL.GetAllCountries();

            return Ok(response);
        }

        [HttpGet("HelpOption/GetAll")]
        public IActionResult GetAllContactUsHelpOptions()
        {
            var response = _lookUpBLL.GetAllContactUsHelpOptions();

            return Ok(response);
        }

        [HttpGet("Industry/GetAll")]
        public IActionResult GetAllIndustries()
        {
            var response = _lookUpBLL.GetAllIndustries();

            return Ok(response);
        }

        [HttpGet("CompanySize/GetAll")]
        public IActionResult GetAllCompanySize()
        {
            var response = _lookUpBLL.GetAllCompanySize();

            return Ok(response);
        }

        [HttpGet("DexefBranch/GetAll")]
        public IActionResult GetAllDexefBranches()
        {
            var response = _lookUpBLL.GetAllDexefBranches();

            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> SendRequest(NewContactRequestDto newRequest)
        {
            var result = await _contactUsBLL.SendRequestAsync(newRequest);

            return Ok(result);
        }

        [HttpPost("TestEmail")]
        public async Task<IActionResult> SendEmail()
        {
            var result = await _notificationBLL.SendEmailAsync(EmailProvider.Smtp, new MailContent
            {
                FromEmail = "mohamed.swilam@dexef.net",
                Body = "Call api from crm plugin after create account.",
                Subject = "Crm Plugin",
                DisplayName = "Dexef Crm",
                ToEmails = new List<string>
                {
                    "jackswilam@gmail.com",
                    "omar.siam@dexef.net",
                    "karim.mostafa@dexef.net"
                }
            });

            return Ok(result);
        }

        [HttpPost("TestSMS")]
        public async Task<IActionResult> SendSms()
        {
            var smsResult = await _notificationBLL.SendSmsAsync(SmsProvider.SmsMisr,
                                                    new SmsContent
                                                    {
                                                        From = _authSetting.Sms.SmsTemplate.SmsMisrTemplate.FromPhone,
                                                        ToPhones = new List<string> { "01004662373" },
                                                        LanguageId = (int)SmsMisrLanguageEnum.English,
                                                        MessageContent = _authSetting.Sms.SmsTemplate.SmsMisrTemplate.Content + "888"
                                                    });
            return Ok(smsResult);
        }

        [HttpPost("GetAccountByLeedTest")]
        public async Task<IActionResult> GetAccountByLeedTest(Guid leadId)
        {
            var result = await _crmBll.GetAccountByLeadIdAsync(leadId);

            return Ok(result);
        }

        [HttpPost("TestPdf")]
        public async Task<IActionResult> GenerateQR()
        {
            var invoicePdfDto = new InvoicePdfDto
            {
                //CompanyInfo = new CompanyInfoPdfDto
                //{
                //    Address = "1 Ibrahim basha st, Quessna, Menoufia, Egypt",
                //    TaxReg = "52465635514"
                //},
                CustomerInfo = new CustomerInfoPdfDto
                {
                    ContractSerial = "CUST-5552d6524",
                    Address = "Gamet El Dewal st, Mohandseen, Giza, Egypt",
                    Name = "Said Ragab",
                    TaxReg = "54987965635"
                },
                InvoiceInfo = new InvoiceInfoPdfDto
                {
                    Serial = "INV-325452152",
                    CreateDate = DateTime.UtcNow.AddDays(-25).ToString(),
                    StartDate = DateTime.UtcNow.AddDays(-25).ToString(),
                    EndDate = DateTime.UtcNow.AddDays(5).ToString(),
                    Status = Core.Enums.Invoices.InvoiceStatusEnum.Paid.ToString()
                },
                InvoiceItems = new List<InvoiceItemPdfDto>
                    {
                        new InvoiceItemPdfDto
                        {
                            ProductName = "Dexef Blue - Version 1",
                            PriceLevel = 15,
                            SubscriptionType = "Montly",
                            Discount = 25,
                            NetPrice = 1570,
                            CurrencyName = "EGP"
                        },
                    },
                InvoiceSummary = new InvoiceSummaryPdfDto
                {
                    Subtotal = 1570,
                    VatPercentage = 15,
                    VatValue = 320,
                    Total = 1250,
                    CurrencyName = "EGP"
                }
            };

            var qrBase64 = _pdfGeneratorBLL.GenrateQrImagePath(invoicePdfDto);

            return Ok(qrBase64);
        }
    }
}
