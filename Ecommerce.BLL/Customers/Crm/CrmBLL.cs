using AutoMapper;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Pdfs;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Crm.Tickets;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Enums.Tickets;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Crm;
using Ecommerce.DTO.Customers.Crm.Tickets;
using Ecommerce.DTO.Customers.Ticket;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Settings.Crm;
using Ecommerce.Reports.Templts;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using Entity = Ecommerce.Core.Entities;
using NewtonSoft = Newtonsoft.Json;

namespace Ecommerce.BLL.Customers.Crm
{
    public class CrmBLL : BaseBLL, ICrmBLL
    {
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CrmSetting _crmSetting;
        private readonly IRepository<Entity.Invoice> _invoiceRepository;
        //private readonly IRepository<Entity.Ticket> _ticketRepository;
        private readonly IPdfGeneratorBLL _pdfGeneratorBLL;
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly IRepository<Entity.Version> _versionRepository;

        private readonly IBlobFileBLL _blobFileBLL;

        private readonly IUnitOfWork _unitOfWork;
        public CrmBLL(IMapper mapper,
                      IHttpClientFactory httpClient,
                      IOptions<CrmSetting> crmSetting,
                      IPdfGeneratorBLL pdfGeneratorBLL,
                      IRepository<Entity.Invoice> invoiceRepository,
                      INotificationDataBLL notificationDataBLL,
                      IRepository<Core.Entities.Version> versionRepository
// IRepository<Entity.Ticket> ticketRepository
,
                     IBlobFileBLL blobFileBLL,

                     IUnitOfWork unitOfWork)
            : base(mapper)
        {
            _mapper = mapper;
            _httpClientFactory = httpClient;
            _crmSetting = crmSetting.Value;
            _pdfGeneratorBLL = pdfGeneratorBLL;
            _invoiceRepository = invoiceRepository;
            _notificationDataBLL = notificationDataBLL;
            _versionRepository = versionRepository;
            // _ticketRepository = ticketRepository;
            _blobFileBLL = blobFileBLL;

            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreateLeadAsync(CrmNewLeadDto newLead)
        {
            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.CreateLead}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var jsonLead = JsonSerializer.Serialize(newLead);
                httpRequestMessage.Content = new StringContent(jsonLead, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                //var content = await httpResponseMessage.Content.ReadAsStreamAsync();

                //var contentString = await JsonSerializer.DeserializeAsync<string>(content);
                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var content = NewtonSoft.JsonConvert.DeserializeObject<string>(contentStream);

                return content;
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> QualifyLeadAsync(QualifyLeadDto qualifyLeadDto)
        {
            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.QualifyLead}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var jsonQualifyLead = JsonSerializer.Serialize(qualifyLeadDto);
                httpRequestMessage.Content = new StringContent(jsonQualifyLead, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var content = NewtonSoft.JsonConvert.DeserializeObject<string>(contentStream);


                return content;
            }
            catch
            {
                throw;
            }
        }

        public async Task<CrmRetrieveBaseDto> GetAccountByLeadIdAsync(Guid leadId)
        {
            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.GetAccountByLeadId}?leadId={leadId}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var contentString = await httpResponseMessage.Content.ReadAsStringAsync();

                var content = NewtonSoft.JsonConvert.DeserializeObject<CrmRetrieveBaseDto>(contentString);

                return content;
            }
            catch
            {
                throw;
            }
        }

        public async Task<CrmRetrieveBaseDto> GetAccountByIdAsync(Guid accountId)
        {
            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.GetAccountById}?accountId={accountId}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                //var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                //var content = await JsonSerializer.DeserializeAsync<CrmRetrieveBaseDto>(contentStream);
                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var content = NewtonSoft.JsonConvert.DeserializeObject<CrmRetrieveBaseDto>(contentStream);

                return content;
            }
            catch
            {
                throw;
            }
        }

        #region Tickets.

        public async Task<IResponse<string>> GetSubjectsLookupAsync()
        {
            var response = new Response<string>();

            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.GetSubjectsLookup}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var content = await httpResponseMessage.Content.ReadAsStringAsync();

                return response.CreateResponse(content);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<string>> GetCustomerTicketsAsync(TicketFilterDto ticketFilter, Guid customerId)
        {
            var response = new Response<string>();

            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.GetCustomerTickets}?customerId={customerId}&pageNumber={ticketFilter.PageNumber}&pageSize={ticketFilter.PageSize}&statusId={ticketFilter.StatusId}&search={ticketFilter.Search}&fromDate={ticketFilter.FromDate}&toDate={ticketFilter.ToDate}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var content = await httpResponseMessage.Content.ReadAsStringAsync();

                return response.CreateResponse(content);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<TicketDto>> GetTicketInfoAsync(Guid ticketId)
        {
            var response = new Response<TicketDto>();

            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.GetTicketInfo}?ticketId={ticketId}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();

                var ticketInfoDto = NewtonSoft.JsonConvert.DeserializeObject<TicketDto>(contentStream);

                if (!string.IsNullOrEmpty(ticketInfoDto.ProductNumber))
                {
                    var version = await _versionRepository.GetAsync(v => v.ProductCrmId.Equals(ticketInfoDto.ProductNumber));

                    if (version is not null)
                        ticketInfoDto.Product = version.Title;
                }

                return response.CreateResponse(ticketInfoDto);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<string>> GetTicketMessagesAsync(Guid ticketId, int pageNumber, int pageSize)
        {
            var response = new Response<string>();

            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.GetTicketMessages}?ticketId={ticketId}&pageNumber={pageNumber}&pageSize={pageSize}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var content = await httpResponseMessage.Content.ReadAsStringAsync();

                return response.CreateResponse(content);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<string>> GetTicketLogsAsync(Guid ticketId)
        {
            var response = new Response<string>();

            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.GetTicketLogs}?ticketId={ticketId}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var content = await httpResponseMessage.Content.ReadAsStringAsync();

                return response.CreateResponse(content);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }


        public async Task<IResponse<RetrieveNewTicketDto>> CreateTicketAsync(NewTicketNotificationDto newTicket)
        {
            var response = new Response<RetrieveNewTicketDto>();

            try
            {
                var ticketValidation = await new NewTicketDtoValidator().ValidateAsync(newTicket);

                if (!ticketValidation.IsValid)
                {
                    return response.CreateResponse(ticketValidation.Errors);
                }

                if (newTicket.Topic == (int)TicketTopicEnum.Billing)
                {
                    var pdfResponse = await GenerateInvoicePdfAsync(newTicket.InvoiceId);

                    if (!pdfResponse.IsSuccess)
                        return response.AppendErrors(pdfResponse.Errors);

                    newTicket.HasAttachment = true;
                    newTicket.Attachment = new MessageAttachmentDto
                    {
                        FileBase64 = pdfResponse.Data.InvoiceBase64,
                        FileName = $"{pdfResponse.Data.Serial}.pdf",
                        MimeType = "application/octet-stream"
                    };
                }

                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.CreateTicket}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var jsonTicket = JsonSerializer.Serialize(newTicket);
                httpRequestMessage.Content = new StringContent(jsonTicket, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var contentString = await httpResponseMessage.Content.ReadAsStringAsync();

                var createdTicketDto = NewtonSoft.JsonConvert.DeserializeObject<RetrieveNewTicketDto>(contentString);

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = newTicket.userId;
                _notificationItem.IsAdminSide = true;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.CreateTicket;
                _notificationItem.TicketId = createdTicketDto.Id.ToString();
                _notificationItem.TicketRefrences = JsonSerializer.Serialize(
                 new
                 {
                     TicketNo = createdTicketDto.Serial,
                     SenderName = newTicket.SenderName,
                     Id = createdTicketDto.Id,
                 });
                await _notificationDataBLL.CreateAsync(_notificationItem);

                return response.CreateResponse(createdTicketDto);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<RetrieveNewTicketDto>> SendTicketMessageAsync(NewTicketMessageNotificationDto newMessage)
        {
            var response = new Response<RetrieveNewTicketDto>();

            try
            {
                var ticketValidation = await new NewTicketMessageDtoValidator().ValidateAsync(newMessage);

                if (!ticketValidation.IsValid)
                {
                    return response.CreateResponse(ticketValidation.Errors);
                }

                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.SendTicketMessage}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var jsonTicket = JsonSerializer.Serialize(newMessage);
                httpRequestMessage.Content = new StringContent(jsonTicket, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var contentString = await httpResponseMessage.Content.ReadAsStringAsync();

                var createdTicket = NewtonSoft.JsonConvert.DeserializeObject<RetrieveNewTicketDto>(contentString);

                //push new notification db
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = newMessage.userId;
                _notificationItem.AdminId = newMessage.AdminId;
                _notificationItem.TicketId = createdTicket.Id.ToString();
                _notificationItem.TicketRefrences = JsonSerializer.Serialize(
                 new
                 {
                     TicketNo = createdTicket.Serial,
                     SenderName = newMessage.SenderName,
                 });
                _notificationItem.IsAdminSide = !newMessage.IsAdmin;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.TicketReply;
                await _notificationDataBLL.CreateAsync(_notificationItem);

                return response.CreateResponse(createdTicket);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> CancelTicketAsync(TicketBaseDto ticket)
        {
            var response = new Response<bool>();

            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.CancelTicket}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var jsonTicket = JsonSerializer.Serialize(ticket);
                httpRequestMessage.Content = new StringContent(jsonTicket, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var content = await httpResponseMessage.Content.ReadAsStreamAsync();

                var contentString = await JsonSerializer.DeserializeAsync<bool>(content);

                return response.CreateResponse(contentString);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> CloseTicketAsync(CloseTicketDto ticket)
        {
            var response = new Response<bool>();

            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.CloseTicket}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var jsonTicket = JsonSerializer.Serialize(ticket);
                httpRequestMessage.Content = new StringContent(jsonTicket, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var content = await httpResponseMessage.Content.ReadAsStreamAsync();

                var contentString = await JsonSerializer.DeserializeAsync<bool>(content);

                return response.CreateResponse(contentString);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> ReopenTicketAsync(TicketBaseDto ticket)
        {
            var response = new Response<bool>();

            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.ReopenTicket}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var jsonTicket = JsonSerializer.Serialize(ticket);
                httpRequestMessage.Content = new StringContent(jsonTicket, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var content = await httpResponseMessage.Content.ReadAsStreamAsync();

                var contentString = await JsonSerializer.DeserializeAsync<bool>(content);

                return response.CreateResponse(contentString);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> AssignAgnetToTicketAsync(AssignAgnetToTicket assignAgnetToTicket)
        {
            var response = new Response<bool>();

            try
            {
                var url = $"{_crmSetting.BaseUrl}/{_crmSetting.Endpoint.AssignAgnetToTicket}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                var jsonTicket = JsonSerializer.Serialize(assignAgnetToTicket);
                httpRequestMessage.Content = new StringContent(jsonTicket, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var failException = await httpResponseMessage.Content.ReadAsStringAsync();
                    return null;
                }

                var content = await httpResponseMessage.Content.ReadAsStreamAsync();

                var contentString = await JsonSerializer.DeserializeAsync<bool>(content);

                return response.CreateResponse(contentString);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        private async Task<IResponse<BillingInvoiceDto>> GenerateInvoicePdfAsync(int invoiceId)
        {
            var response = new Response<BillingInvoiceDto>();

            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

            if (invoice is null)
                return response.CreateResponse(MessageCodes.NotFound, nameof(invoiceId));

            var invoiceDto = _mapper.Map<InvoicePdfOutputDto>(invoice);
            var lang = Thread.CurrentThread.CurrentCulture.Name.Contains("ar") ? "Ar" : "En";
            var pdfResult = await _pdfGeneratorBLL.GenerateInvoicePdfAsync(invoiceDto, false, lang);

            if (!pdfResult.IsSuccess)
                return response.AppendErrors(pdfResult.Errors);

            var billingDto = new BillingInvoiceDto
            {
                Serial = invoice.Serial,
                InvoiceBase64 = pdfResult.Data
            };

            return response.CreateResponse(billingDto);
        }

        public async Task<IResponse<List<GetSubjectsDto>>> GetSubjectsAsync()
        {
            var response = new Response<List<GetSubjectsDto>>();
            try
            {
                var Subjects = new List<GetSubjectsDto>
                {
                    new GetSubjectsDto { id = Guid.NewGuid()},
                    new GetSubjectsDto { id = Guid.NewGuid() }
                };
                return response.CreateResponse(Subjects);
            }
            catch(Exception e)
            {
                return response.CreateResponse(e);
            }


        }
        #endregion
    }
}
