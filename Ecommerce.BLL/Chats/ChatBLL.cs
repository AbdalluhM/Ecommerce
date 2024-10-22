using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Files;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Auth;
using Ecommerce.Core.Enums.Chat;
using Ecommerce.Core.Enums.Files;
using Ecommerce.Core.Enums.Tickets;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.ChatMessage;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Settings.Enviroment;
using Ecommerce.DTO.Settings.Files;
using Ecommerce.Repositroy.Base;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace Ecommerce.BLL.Chats
{
    public class ChatBLL : BaseBLL, IChatBLL
    {
        private readonly IMapper _mapper;
        private readonly IChatHub _chatHub;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobFileBLL _blobFileBll;
        private readonly FileStorageSetting _fileSetting;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IRepository<FileType> _fileTypeRepository;

        public ChatBLL(IMapper mapper, IChatHub chatHub , IRepository<FileType> fileTypeRepository, IUnitOfWork unitOfWork, IRepository<ChatMessage> chatMessageRepository, IOptions<FileStorageSetting> fileSetting, IWebHostEnvironment webHostEnvironment, IBlobFileBLL blobFileBll) : base(mapper)
        {
            _chatHub=chatHub;
            if (fileSetting is not null)
                _fileSetting = fileSetting.Value;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _chatMessageRepository = chatMessageRepository;
            _blobFileBll = blobFileBll;
            _fileTypeRepository = fileTypeRepository;
        }
        public async Task<IResponse<PagedResultDto<MessageResultDto>>> GetPagedListAsync(FilterChatDto pagedDto)
        {

            var response = new Response<PagedResultDto<MessageResultDto>>();

            try
            {


                var x = _chatMessageRepository.GetAll().ToList();
                var orderExpre = GetOrderExpressionNested<ChatMessage>(pagedDto.SortBy ?? "SendTime");
                var result
                    = GetPagedList<MessageResultDto, ChatMessage, dynamic>(pagedDto: pagedDto,
                    repository: _chatMessageRepository,
                    orderExpression: orderExpre,
                    searchExpression: i =>(i.TicketId == pagedDto.TicketId)&& (!string.IsNullOrEmpty(pagedDto.SearchTerm) ? i.Message.Contains(pagedDto.SearchTerm) : true),
                    sortDirection: pagedDto.SortingDirection, /*nameof(SortingDirection.DESC)*/
                    disableFilter: true);
                return response.CreateResponse(result);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
        }

        public async Task<IResponse<bool>> MarkMessageToBeReceived(int id)
        {
            var response = new Response<bool>();

            try
            {
                _chatMessageRepository.GetById(id).ReceivedTime = DateTime.UtcNow;
                _unitOfWork.Commit();

                return response.CreateResponse(true);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }


        }
        public async Task<IResponse<bool>> MarkMessageToBeRead(int ticketId, bool isCustomer)
        {
            var response = new Response<bool>();

            try
            {
             _chatMessageRepository.Where(e => e.IsCustomer && e.TicketId == ticketId && e.ReadTime == null)
                    .ToList().Select(e => e.ReadTime = DateTime.UtcNow).ToList();

                _unitOfWork.Commit();

                return response.CreateResponse(true);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }


        }
        public async Task<IResponse<int>> GetCountOfUnRead(int currentId, bool isCustomer)
        {
            var response = new Response<int>();

            try
            {
              var count = _chatMessageRepository.Where(e => e.IsCustomer && e.ReadTime == null).Count();
              
                return response.CreateResponse(count);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }


        }
        public async Task<IResponse<bool>> SendMessage(MessageDto inputDto)
        {
            var response = new Response<bool>();

            try
            {
                var Message = _mapper.Map<ChatMessage>(inputDto);
                Message.SenderId = inputDto.CustomerId;
                var result = await _chatMessageRepository.AddAsync(Message);


                await _unitOfWork.CommitAsync();
                var messageResult = _mapper.Map<MessageResultDto>(result);

                if (inputDto.HasAttachment)
                {
                    foreach (var file in inputDto.Files)
                    {
                        var singleFilebase = new SingleFilebaseDto();
                        singleFilebase.FileDto = new FileDto();
                        singleFilebase.FileDto.ContainerName = _fileSetting.Files.Customers.Ticket.ContainerName;
                        singleFilebase.FileDto.FileBaseDirectory = _webHostEnvironment != null
                            ? _webHostEnvironment.ContentRootPath
                            : AppContext.BaseDirectory;
                        singleFilebase.FileDto.FilePath = _fileSetting.Files.Customers.Ticket.Path;
                        singleFilebase.FileDto.File = file;
                        var fileStorage = await _blobFileBll.UploadFileAsync(singleFilebase, entityId: result.Id,
                            nameId: (int)TableNameEnum.ChatMessage);
                        if (fileStorage.IsSuccess)
                        {
                            messageResult.MediaUrl.Add(new MediaDto()
                            {
                                Url = Path.Combine( _fileSetting.BlobBaseUrl,fileStorage.Data.FullPath).Replace("\\", "/"),
                                Extention = fileStorage.Data.Extension,
                                FileType = _fileTypeRepository.GetById(fileStorage.Data.FileTypeId)?.Name,
                            });

                        }

                    }
                }

                await _chatHub.SendMessage(messageResult, inputDto.UserType);


                return response.CreateResponse(true);
            }
            catch (Exception e)
            {
                return response.CreateResponse(e);

            }


        }

        public async Task<IResponse<string>> DownLoadChat(int ticketId)
        {
            var response = new Response<string>();
            try
            {
                var chat =await _chatMessageRepository.GetManyAsync(e => e.TicketId == ticketId);
                if (chat == null)
                    return response.CreateResponse(DXConstants.MessageCodes.NotFound, ticketId.ToString());
                var mapped = _mapper.Map<List<MessageResultDto>>(chat);

                var result = await CreatePdfAndReturnBase64(mapped);
                return response.CreateResponse(result);

            }
            catch (Exception e)
            {
                return response.CreateResponse(e);
            }
           
        }
        private async Task<string> CreatePdfAndReturnBase64(List<MessageResultDto> messages)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
            XPoint position = new XPoint(50, 50);
            const double Margin = 50;
            const double LineHeight = 20;
            const double ImageHeight = 100;

            gfx.DrawString("Messages", font, XBrushes.Black, position);
            position.Y += LineHeight;

            foreach (var message in messages)
            {
                string messageText = $"{message.SendTime}: {message.Name} - {message.Message}";
                gfx.DrawString(messageText, font, XBrushes.Black, position);
                position.Y += LineHeight;

                if (position.Y > page.Height - Margin)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    position.Y = Margin; 
                }

                if (message.HasAttachment)
                {
                    foreach (var media in message.MediaUrl)
                    {
                        if (media.FileType.ToLower() == nameof(FileTypeEnum.image))
                        {
                            using (var client = new HttpClient())
                            {
                                var imageStream = await client.GetStreamAsync(media.Url);
                                using (var xImage = XImage.FromStream(() => imageStream))
                                {

                                    if (position.Y + ImageHeight > page.Height - Margin)
                                    {
                                        page = document.AddPage();
                                        gfx = XGraphics.FromPdfPage(page);
                                        position.Y = Margin;
                                    }

                                    gfx.DrawImage(xImage, position.X, position.Y, 100, 100);
                                    position.Y += ImageHeight + LineHeight;
                                }
                            }
                        }
                    }
                }

                
                if (position.Y > page.Height - Margin)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    position.Y = Margin; 
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, false);
                byte[] pdfBytes = stream.ToArray();
                return Convert.ToBase64String(pdfBytes);
            }
        }

    }
}
