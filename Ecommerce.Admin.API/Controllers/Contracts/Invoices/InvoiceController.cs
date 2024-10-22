using AutoMapper;
using Hangfire;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.API.Attributes;
using Ecommerce.BLL.Contracts.Invoices;
using Ecommerce.BLL.Customers.Invoices;
using Ecommerce.BLL.Customers.Invoices.Job;
using Ecommerce.Core.Enums.Roles;
using Ecommerce.DTO.Customers.Invoices;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;
using static Ecommerce.DTO.Customers.CustomerDto;

namespace Ecommerce.API.Controllers.Contracts.Invoices
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : BaseAdminController
    {
        #region Fields

        private readonly IAdminInvoiceBLL _adminInvoiceBLL;
        private readonly IInvoiceBLL _invoiceBLL;
        private readonly IInvoiceJobBLL _invoiceJobBLL;
        private readonly IMapper _mapper;
        #endregion
        #region Constructor

        public InvoiceController( IAdminInvoiceBLL adminInvoiceBLL,
                                 IHttpContextAccessor httpContextAccessor,
                                 IMapper mapper,
                                 IInvoiceBLL invoiceBLL,
                                 IInvoiceJobBLL invoiceJobBLL ) : base(httpContextAccessor)
        {
            _adminInvoiceBLL = adminInvoiceBLL;
            _mapper = mapper;
            _invoiceBLL = invoiceBLL;
            _invoiceJobBLL = invoiceJobBLL;
        }


        #endregion
        #region Actions


        [Route("GetContractInvoicesPagedList")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.READ)]
        public async Task<IActionResult> GetContractInvoicesPagedList([FromQuery] InvoiceFilterDto inputDto)
        {
            var input = _mapper.Map<FilterInvoiceByCountry>(inputDto);
            input.EmployeeId = CurrentEmployeeId;
            var output = await _adminInvoiceBLL.GetAllInvoicesPagedlist(input);
            return Ok(output);
        }

        [Route("GetAllInvoices")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllInvoices()
        {
            var output = await _adminInvoiceBLL.GetAllInvoices(CurrentEmployeeId);
            return Ok(output);
        }
        [HttpGet("GetInvoiceById")]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.READ)]
        public async Task<IActionResult> Get(int id)
        {
            var output = await _adminInvoiceBLL.GetInvoiceById(id);

            return Ok(output);

        }
        [Route("GetVersionsOrAddOnsByCustomer")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.READ)]
        public async Task<IActionResult> GetVersionsOrAddOnsByCustomer([FromQuery] GetCustomerDefaultTaxApiInputDto inputDto)
        {

            var input = _mapper.Map<GetCustomerDefaultTaxInputDto>(inputDto);

            var output = await _adminInvoiceBLL.GetVersionAndAddOnAsync(input);
            return Ok(output);
        }
        [Route("GetCustomerVersions")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.READ)]
        public async Task<IActionResult> GetCustomerVersions([FromQuery] GetCustomerDefaultTaxApiInputDto inputDto)
        {

            var input = _mapper.Map<GetCustomerDefaultTaxInputDto>(inputDto);

            var output = await _adminInvoiceBLL.GetCutomerVersionsAsync(input);
            return Ok(output);
        }
        [Route("GetAllAddOnCanPurshased")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllAddOnCanPurshased(int customerId, int versonSubscriptionId)
        {
            var output = await _adminInvoiceBLL.GetAddOnCanPurshasedAsync(customerId, versonSubscriptionId);
            return Ok(output);
        }
        [Route("GetAllVersionsCanPurshased")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.READ)]
        public IActionResult GetAllVersionsCanPurshased(int customerId)
        {
            var output = _adminInvoiceBLL.GetVersionCanPurshased(customerId);
            return Ok(output);
        }
        [Route("GetAllCustomer")]
        [HttpGet]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.READ)]
        public async Task<IActionResult> GetAllCustomer()
        {
            var output = await _adminInvoiceBLL.GetAllActiveCustomerAsync(CurrentEmployeeId);
            return Ok(output);
        }
        [Route("CreateSupportInvoice")]
        [HttpPost]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateSupportInvoice(PaymentDetailsInputDto inputDto, int customerId)
        {
            inputDto.CustomerId = customerId;
            var output = await _adminInvoiceBLL.CreateSupportInvoice(inputDto, CurrentEmployeeId);
            return Ok(output);
        }
        [Route("CreateInvoice")]
        [HttpPost]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.CREATE)]
        public async Task<IActionResult> CreateInvoice(CreateAdminInvoiceInputDto inputDto, int customerId)
        {
            var input = _mapper.Map<PaymentDetailsInputDto>(inputDto);
            input.CustomerId = customerId;
            var output = await _adminInvoiceBLL.CreateInvoice(input, CurrentEmployeeId);
            return Ok(output);
        }
        [Route("UpdateInvoice")]
        [HttpPost]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.UPDATE)]
        public async Task<IActionResult> UpdateInvoice(UpdateAdminInvoiceInputDto inputDto, int customerId)
        {
            var input = _mapper.Map<PaymentDetailsInputDto>(inputDto);
            input.CustomerId = customerId;
            var output = await _adminInvoiceBLL.UpdateInvoice(input);
            return Ok(output);
        }
        [Route("AssignInvoice")]
        [HttpPost]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.UPDATE)]
        public async Task<IActionResult> AssignInvoice(List<int> ids)
        {
            var output = await _adminInvoiceBLL.AssignInvoicesToCustomer(ids);
            return Ok(output);
        }
        [Route("DeleteInvoice")]
        [HttpPost]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteInvoice(List<int> ids)
        {
            var output = await _adminInvoiceBLL.DeleteInvoice(ids);
            return Ok(output);
        }
        [Route("CancelInvoice")]
        [HttpPost]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.UPDATE)]
        public async Task<IActionResult> CancelInvoice(CancelInvoiceInputDto inputDto)
        {
            var output = await _adminInvoiceBLL.CancelInvoice(inputDto);
            return Ok(output);
        }

        [HttpPost("DownloadInvoices")]
        //[DxAuthorize(PagesEnum.Invoices, ActionsEnum.READ)]
        public async Task<IActionResult> DownloadInvoicesAsync(IEnumerable<int> invoicesIds)
        {
            var str = CurrentEmployeeLanguage;
            var result = await _adminInvoiceBLL.DownloadInvoicesAsync(invoicesIds, CurrentEmployeeLanguage);

            return Ok(result);
        }

        [HttpPost("GenerateInvoiceQrCode")]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.CREATE)]
        public async Task<IActionResult> GenerateInvoiceQrCodeAsync(int invoiceId)
        {
            var result = await _invoiceBLL.GenerateInvoiceQrCodeAsync(invoiceId);

            return Ok(result);
        }

        [HttpPost("RefundInvoiceCash")]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.UPDATE)]
        public async Task<IActionResult> RefundInvoiceCashAsync(int invoiceId)
        {
            var result = await _adminInvoiceBLL.RefundInvoiceCashAsync(invoiceId, CurrentEmployeeId);

            return Ok(result);
        }
        [HttpPost("PayInvoiceCash")]
        [DxAuthorize(PagesEnum.Invoices, ActionsEnum.UPDATE)]
        public async Task<IActionResult> PayInvoiceCashByAdminAsync(int invoiceId)
        {
            var result = await _adminInvoiceBLL.PayInvoiceByAdminAsync(invoiceId,CurrentEmployeeId);

            return Ok(result);
        }
        #endregion

        #region CRM
        [Route("CreateInvoicesForCRMAccounts")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreateInvoicesForCRMAccounts( /*[FromQuery] RenewOldInvoiceInputDto inputDto*/ )
        {
             BackgroundJob.Enqueue(( ) =>  _invoiceJobBLL.RenewOldInvoicesAutoAsync(/*inputDto*/));
            return Ok();
        }

        #endregion
    }
}