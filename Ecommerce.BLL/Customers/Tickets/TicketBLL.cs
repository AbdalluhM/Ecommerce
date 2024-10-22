using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Pdfs;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Crm.Tickets;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.Core.Enums.Tickets;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Crm.Tickets;
using Ecommerce.DTO.Customers.Ticket;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.Reports.Templts;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Customers.Tickets
{
    public class TicketBLL : BaseBLL, ITicketBLL
    {
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPdfGeneratorBLL _pdfGeneratorBLL;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<FileStorage> _fileStorageRepository;
        private readonly IRepository<TableName> _tableNameRepository;
        private readonly IBlobFileBLL _blobFileBLL;
        private readonly IFileBLL _fileBLL;
        private readonly FileStorageSetting _fileSetting;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public TicketBLL(IRepository<FileStorage> fileStorageRepository, INotificationDataBLL notificationDataBLL, IUnitOfWork unitOfWork, IMapper mapper, IPdfGeneratorBLL pdfGeneratorBLL, IRepository<Invoice> invoiceRepository, IRepository<Ticket> ticketRepository, IRepository<TableName> tableNameRepository, IBlobFileBLL blobFileBLL, IFileBLL fileBLL, IOptions<FileStorageSetting> fileSetting, IWebHostEnvironment webHostEnvironment) : base(mapper)

        {
            _notificationDataBLL = notificationDataBLL;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pdfGeneratorBLL = pdfGeneratorBLL;
            _invoiceRepository = invoiceRepository;
            _ticketRepository = ticketRepository;
            _tableNameRepository = tableNameRepository;
            _blobFileBLL = blobFileBLL;
            _fileBLL = fileBLL;
            _webHostEnvironment = webHostEnvironment;
            if (fileSetting is not null)
                _fileSetting = fileSetting.Value;
            _fileStorageRepository = fileStorageRepository;
        }

        public async Task<IResponse<RetrieveNewTicketDto>> CreateTicketAsync(CreateTicketDto newTicket, List<SingleFilebaseDto> files)
        {
            var response = new Response<RetrieveNewTicketDto>();
            try
            {
                var ticketValidation = await new NewTicketValidator().ValidateAsync(newTicket);
                if (!ticketValidation.IsValid)
                {
                    return response.CreateResponse(ticketValidation.Errors);
                }

                // Check is billing and generate invoice
                if (newTicket.Topic == (int)TicketTopicEnum.Billing)
                {

                    var invoice = await _invoiceRepository.GetByIdAsync(newTicket.InvoiceId ?? 0);
                    if (invoice == null)
                    {
                        return response.CreateResponse(MessageCodes.NotFound, nameof(Invoice));
                    }
                    var pdfResponse = await GenerateInvoicePdfAsync(newTicket.InvoiceId ?? 0);

                    if (!pdfResponse.IsSuccess)
                        return response.AppendErrors(pdfResponse.Errors);

                    newTicket.HasAttachment = true;
                    byte[] bytes = Convert.FromBase64String(pdfResponse.Data.InvoiceBase64);
                    MemoryStream stream = new MemoryStream(bytes);

                    var file = new FormFile(stream, 0, bytes.Length, pdfResponse.Data.Serial, pdfResponse.Data.Serial)
                    {
                        Headers = new HeaderDictionary(),

                        ContentType = MediaTypeNames.Application.Pdf
                    };
                    // file.ContentType = getFileContentType("pdf");

                    files.Add(new SingleFilebaseDto
                    {
                        FileDto = new FileDto
                        {
                            File = file,
                            FilePath = _fileSetting.Files.Customers.Ticket.Path,
                            ContainerName = _fileSetting.Files.Customers.Ticket.ContainerName,
                            FileBaseDirectory = _webHostEnvironment != null ? _webHostEnvironment.ContentRootPath : AppContext.BaseDirectory
                        }
                    });
                }


                // Add new ticket to database
                var mappedTicket = _mapper.Map<Ticket>(newTicket);
                mappedTicket.StatusId = (int)TicketTypesEnum.Active;
                var result = await _ticketRepository.AddAsync(mappedTicket);
                await _unitOfWork.CommitAsync();



                //upload files to blob storage and database
                foreach (var file in files)
                {
                    var storedFile = await _blobFileBLL.UploadFileAsync(file, entityId: result.Id, nameId: (int)TableNameEnum.Ticket);
                }


                var createdTicketDto = new RetrieveNewTicketDto() { Id = result.Id, Serial = "" };
                var _notificationItem = new GetNotificationForCreateDto();
                _notificationItem.CustomerId = newTicket.CustomerId;
                _notificationItem.IsAdminSide = true;
                _notificationItem.IsCreatedBySystem = false;
                _notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.CreateTicket;
                _notificationItem.TicketId = createdTicketDto.Id.ToString();
                _notificationItem.TicketRefrences = JsonSerializer.Serialize(
                 new
                 {
                     TicketNo = createdTicketDto.Serial,
                     newTicket.SenderName,
                     createdTicketDto.Id,
                 });
                await _notificationDataBLL.CreateAsync(_notificationItem);
                await _unitOfWork.CommitAsync();
                return response.CreateResponse(createdTicketDto);
            }

            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }

        }


        public async Task<IResponse<bool>> CloseTicket(int id)
        {
            var response = new Response<bool>();

            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(id);
                if (ticket == null)
                    return response.CreateResponse(MessageCodes.NotFound, id.ToString());

                ticket.StatusId = (int)TableStatusEnum.Close;
                await _unitOfWork.CommitAsync();
                return response.CreateResponse(true);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }

        }
        public async Task<IResponse<bool>> CancelTicket(int id)
        {
            var response = new Response<bool>();

            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(id);
                if (ticket == null)
                    return response.CreateResponse(MessageCodes.NotFound, id.ToString());

                ticket.StatusId = (int)TableStatusEnum.Canceled;
                await _unitOfWork.CommitAsync();
                return response.CreateResponse(true);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }

        }
        public async Task<IResponse<List<GetTicketFilterDateDto>>> GetTickets(FilteredResultRequestDto pagedDto, int customerId)
        {
            var response = new Response<List<GetTicketFilterDateDto>>();
            var orderExpression = GetOrderExpression<Ticket>(pagedDto.SortBy);

            var tickets = GetPagedList<GetTicketDto, Ticket, dynamic>(pagedDto: pagedDto,
                repository: _ticketRepository,
                orderExpression: orderExpression,
                searchExpression: i => i.CustomerId == customerId,
                sortDirection: pagedDto.SortingDirection,/*nameof(SortingDirection.DESC)*/
                disableFilter: true);

            var t = tickets.Items.GroupBy(x => x.CreateDate.Date)
                .Select(e => new GetTicketFilterDateDto
                {
                    Date = e.Key,
                    Tickets = e.ToList()
                });

            return response.CreateResponse(t.ToList());
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


    }
}
