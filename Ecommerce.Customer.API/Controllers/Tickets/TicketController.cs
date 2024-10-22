using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Customers.Crm;
using Ecommerce.DTO.Customers.Crm.Tickets;
using System;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Core.Consts.Auth;
using Ecommerce.BLL.Responses;
using Microsoft.AspNetCore.Http;
using Ecommerce.Core.Enums.Settings.Files;
using Ecommerce.DTO.Contracts.Licenses.Inputs;
using DevExpress.Office.Utils;
using Ecommerce.DTO.Files;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Ecommerce.DTO.Settings.Files;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Customers.Tickets;
using Ecommerce.DTO.Customers.Ticket;
using Ecommerce.DTO.Paging;

namespace Ecommerce.Customer.API.Controllers.Tickets
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : BaseCustomerController
    {
        private readonly ICrmBLL _crmBLL;
        private readonly IMapper _mapper;
        private readonly ITicketBLL _ticketBLL;

        public TicketController(IWebHostEnvironment webHostEnvironment, IOptions<FileStorageSetting> fileOptions, ICrmBLL crmBLL, IMapper mapper,
                              IHttpContextAccessor httpContextAccessor, ITicketBLL ticketBLL) : base(fileOptions, webHostEnvironment, httpContextAccessor)
        {
            _crmBLL = crmBLL;
            _mapper = mapper;
            _ticketBLL = ticketBLL;
        }

        [HttpGet("GetSubjectsLookup")]
        public async Task<ActionResult> GetSubjectsLookup()
        {
            var result = await _crmBLL.GetSubjectsLookupAsync();

            return Ok(result);
        }

        [HttpGet("GetSubjects")]
        public async Task<ActionResult> GetSubjects()
        {
            var result = await _crmBLL.GetSubjectsAsync();

            return Ok(result);
        }

        [HttpGet("GetCustomerTickets")]
        public async Task<ActionResult> GetCustomerTickets([FromQuery] TicketFilterDto ticketFilter)
        {
            if (CrmId.HasValue)
                return Ok(await _crmBLL.GetCustomerTicketsAsync(ticketFilter, CrmId.Value));
            else
            {
                var output = new Response<bool>();
                output.IsSuccess = false;
                return Ok(output.CreateResponse());
            }
        }

        [HttpGet("GetTicketInfo")]
        public async Task<ActionResult> GetTicketInfo(Guid ticketId)
        {
            var result = await _crmBLL.GetTicketInfoAsync(ticketId);

            return Ok(result);
        }

        [HttpGet("GetTicketMessages")]
        public async Task<ActionResult> GetTicketMessages(Guid ticketId, int pageNumber, int pageSize)
        {
            var result = await _crmBLL.GetTicketMessagesAsync(ticketId, pageNumber, pageSize);

            return Ok(result);
        }

        [HttpGet("GetTicketLogs")]
        public async Task<ActionResult> GetTicketLogs(Guid ticketId)
        {
            var result = await _crmBLL.GetTicketLogsAsync(ticketId);

            return Ok(result);
        }

        [HttpPost("CreateTicket")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<ActionResult> CreateTicket(NewTicketDto newTicket)
        {


            if (CrmId.HasValue)
            {
                newTicket.CustomerId = CrmId.Value;
                var mappedInput = _mapper.Map<NewTicketNotificationDto>(newTicket);
                mappedInput.IsAdmin = false;
                mappedInput.userId = CurrentUserId;
                return Ok(await _crmBLL.CreateTicketAsync(mappedInput));
            }
            else
            {
                var output = new Response<bool>();
                output.IsSuccess = false;
                output.AppendError(new TErrorField() { Message = "User Has no CrmValue" });
                return Ok(output.CreateResponse());
            }
        }

        [HttpPost("Create")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<ActionResult> CreateTicket_New([FromForm]CreateTicketDto newTicket)
        {
            var file = GetFile(newTicket.Attachment, FilePathEnum.Ticket);
            newTicket.CustomerId = CurrentUserId;
            newTicket.SenderName = CurrentUserName;

            var result = await _ticketBLL.CreateTicketAsync(newTicket, file);
            return Ok(result);

        }
        
        [HttpPost("CloseTicket")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<ActionResult> CloseTicket(int id)
        {
            
            var result = await _ticketBLL.CloseTicket(id);
            return Ok(result);

        }
        
        [HttpPost("CancelTicket")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<ActionResult> CancelTicket(int id)
        {
            var result = await _ticketBLL.CancelTicket(id);
            return Ok(result);

        }

        [HttpPost("SendMessage")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<ActionResult> SendMessage(NewTicketMessageDto newMessage)
        {
            var mappedInput = _mapper.Map<NewTicketMessageNotificationDto>(newMessage);
            mappedInput.IsAdmin = false;
            mappedInput.userId = CurrentUserId;
            var result = await _crmBLL.SendTicketMessageAsync(mappedInput);
            return Ok(result);
        }

        //[HttpPost("CancelTicket")]
        //[Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        //public async Task<ActionResult> CancelTicket(TicketBaseDto ticket)
        //{
        //    var result = await _crmBLL.CancelTicketAsync(ticket);

        //    return Ok(result);
        //}

        //[HttpPost("CloseTicket")]
        //[Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        //public async Task<ActionResult> CloseTicket(CloseTicketDto ticket)
        //{
        //    var result = await _crmBLL.CloseTicketAsync(ticket);

        //    return Ok(result);
        //}

        [HttpPost("ReopenTicket")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<ActionResult> ReopenTicket(TicketBaseDto ticket)
        {
            var result = await _crmBLL.ReopenTicketAsync(ticket);

            return Ok(result);
        }

        [HttpGet("GetPagedList")]
        [Authorize(Policy = PolicyConst.SuspendedUserNotAllowed)]
        public async Task<ActionResult> GetPagedList([FromQuery] FilteredResultRequestDto pagedDto)
        {
            var result =await  _ticketBLL.GetTickets(pagedDto , CurrentUserId);
            return Ok(result);

        }

    }
}
