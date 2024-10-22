using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Ecommerce.BLL.Notifications;
using Ecommerce.DTO;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Notifications;
using System.Threading.Tasks;
using Ecommerce.Helper.String;
using AutoMapper;
using Ecommerce.API.Attributes;
using Ecommerce.Core.Enums.Roles;
using System;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    public class NotificationController : BaseAdminController
    {
        #region Fields

        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly IMapper _mapper;
        #endregion
        #region Constructor

        public NotificationController(ILogger<NotificationController> logger, INotificationDataBLL notificationDataBLL, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(httpContextAccessor)
        {
            _logger = logger;
            _notificationDataBLL = notificationDataBLL;
            _mapper = mapper;
        }
        #endregion
        #region Actions 

        #region Notifications
       
        [HttpGet]
        [Route("GetPagedList")]
        [DxAuthorize(PagesEnum.Notifications, ActionsEnum.READ, ActionsEnum.DEFAULT)]
        public async Task<IActionResult> GetPagedList([FromQuery] FilteredResultRequestDto inputDto)
        {
            var mappedInput = _mapper.Map<NotificationFilteredResultRequestDto>(inputDto);
            mappedInput.IsAdmin = true;
            mappedInput.AdminId = CurrentEmployeeId;
            var output = await _notificationDataBLL.GetPagedListAsync(mappedInput);
            return Ok(output);
        }

        [HttpGet]
        [Route("GetCountOfUnRead")]
        [DxAuthorize(PagesEnum.Notifications, ActionsEnum.READ, ActionsEnum.DEFAULT)]
        public async Task<IActionResult> GetCountOfUnRead()
        {
            NotificationFilterParameterDto _NotificationFilterParameterDto = new NotificationFilterParameterDto();
            _NotificationFilterParameterDto.IsAdmin = true;
            _NotificationFilterParameterDto.AdminId = CurrentEmployeeId;
            var output =await  _notificationDataBLL.GetCountOfUnRead(_NotificationFilterParameterDto);
            return Ok(output);
        }

        
        [HttpGet]
        [Route("MarkAllNotificationsToBeRead")]
        [DxAuthorize(PagesEnum.Notifications, ActionsEnum.UPDATE, ActionsEnum.DEFAULT)]
        public IActionResult MarkAllNotificationsToBeRead()
        {
            NotificationFilterParameterDto _NotificationFilterParameterDto = new NotificationFilterParameterDto();
            _NotificationFilterParameterDto.IsAdmin = true;
            _NotificationFilterParameterDto.AdminId = CurrentEmployeeId;
            var output = _notificationDataBLL.MarkAllNotificationsToBeRead(_NotificationFilterParameterDto);
            return Ok(output);
        }

        [HttpGet]
        [Route("MarkOneNotificationToBeRead")]
        [DxAuthorize(PagesEnum.Notifications, ActionsEnum.UPDATE, ActionsEnum.DEFAULT)]
        public IActionResult MarkOneNotificationToBeRead(int notificationId)
        {
            NotificationFilterParameterDto _NotificationFilterParameterDto = new NotificationFilterParameterDto();
            _NotificationFilterParameterDto.IsAdmin = true;
            _NotificationFilterParameterDto.NotificationId = notificationId;
            _NotificationFilterParameterDto.AdminId = CurrentEmployeeId;
            var output = _notificationDataBLL.MarkOneNotificationToBeRead(_NotificationFilterParameterDto);
            return Ok(output);
        }

        [HttpGet]
        [Route("MarkNotificationToBeHide")]
        [DxAuthorize(PagesEnum.Notifications, ActionsEnum.UPDATE, ActionsEnum.DEFAULT)]
        public IActionResult MarkNotificationToBeHide(int notificationId)
        {
            NotificationFilterParameterDto _NotificationFilterParameterDto = new NotificationFilterParameterDto();
            _NotificationFilterParameterDto.IsAdmin = true;
            _NotificationFilterParameterDto.NotificationId = notificationId;
            _NotificationFilterParameterDto.AdminId = CurrentEmployeeId;
            var output = _notificationDataBLL.MarkOneNotificationToBeHide(_NotificationFilterParameterDto);
            return Ok(output);
        }

        [HttpPost]
        [Route("DeleteRelatedNotification")]
        [DxAuthorize(PagesEnum.Notifications, ActionsEnum.DELETE)]
        public async Task<IActionResult> DeleteRelatedNotification(DeleteNotificationDto _DeleteNotificationDto)
        {
            //delete method of notifications
            var output =await _notificationDataBLL.DeleteRelatedNotificationAsync(_DeleteNotificationDto);
            return Ok(output); 
        }

        #endregion

        #endregion


    }
}
