using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Chats;
using Ecommerce.Core.Enums.Chat;
using Ecommerce.DTO.ChatMessage;
using Ecommerce.DTO.Settings.Files;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers.Chats
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : BaseCustomerController
    {
        private readonly IChatBLL _chatBLL;
        private readonly IMapper _mapper;
        public ChatController(IOptions<FileStorageSetting> fileSetting, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IChatBLL chatBLL, IMapper mapper) :base(fileSetting,  webHostEnvironment,  httpContextAccessor)
        {
            _chatBLL = chatBLL;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] FilterChatDto inputDto)
        {
            var result = await _chatBLL.GetPagedListAsync(inputDto);
            return Ok(result);

        }


        [HttpGet]
        [Route("MarkMessageToBeReceived")]
        public async Task<IActionResult> MarkMessageToBeReceived(int id)
        {
            var result = await _chatBLL.MarkMessageToBeReceived(id);
            return Ok(result);

        }


        [HttpGet]
        [Route("MarkMessageToBeRead")]
        public async Task<IActionResult> MarkMessageToBeRead(int ticketId)
        {
            var result = await _chatBLL.MarkMessageToBeRead(ticketId,true);
            return Ok(result);

        }
        
        [HttpGet]
        [Route("DownLoadChat")]
        public async Task<IActionResult> DownLoadChat(int ticketId)
        {
            var result = await _chatBLL.DownLoadChat(ticketId);
            return Ok(result);

        } 

        [HttpGet]
        [Route("GetCountOfUnRead")]
        public async Task<IActionResult> GetCountOfUnRead()
        {
            var result = await _chatBLL.GetCountOfUnRead(CurrentUserId,true);
            return Ok(result);

        }
        [HttpPost,DisableRequestSizeLimit]
        [RequestSizeLimit(Int32.MaxValue)]
        [Route("SendMessage")]
        public async Task<IActionResult> SendMessage([FromForm] MessageInputDto inputDto)
        {
            var _mapperMessageDto = _mapper.Map<MessageDto>(inputDto);
            _mapperMessageDto.UserType = ChatRecieverTypeEnum.Customer;
            _mapperMessageDto.CustomerId = CurrentUserId;
            var result = await _chatBLL.SendMessage(_mapperMessageDto);
            return Ok(result);

        }
        [HttpPost,DisableRequestSizeLimit]
        [RequestSizeLimit(Int32.MaxValue)]
        [Route("TestChat")]
        public async Task<IActionResult> TestChat([FromForm] MessageInputDto inputDto)
        {
            //test admin send message 
            var _mapperMessageDto = _mapper.Map<MessageDto>(inputDto);
            _mapperMessageDto.UserType = ChatRecieverTypeEnum.Employee;
            _mapperMessageDto.CustomerId = 1;
            var result = await _chatBLL.SendMessage(_mapperMessageDto);
            return Ok(result);

        }
    }
}
