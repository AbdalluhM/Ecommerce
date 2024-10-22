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
namespace Ecommerce.Customer.API.Controllers.Notifications
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class NotifictionController : BaseCustomerController
    {
        #region Fields

        private readonly ILogger<NotifictionController> _logger;
        private readonly INotificationDataBLL _notificationDataBLL;
        private readonly IMapper _mapper;
        #endregion
        #region Constructor

        public NotifictionController(ILogger<NotifictionController> logger,
            INotificationDataBLL notificationDataBLL, IHttpContextAccessor httpContextAccessor,
            IMapper mapper
                             ) : base(httpContextAccessor)
        {
            _logger = logger;
            _notificationDataBLL = notificationDataBLL;
            _mapper = mapper;
        }
        #endregion
        #region Actions 

        #region Notifications Actions

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] FilteredResultRequestDto inputDto)
        {
            var mappedInput = _mapper.Map<NotificationFilteredResultRequestDto>(inputDto);
            mappedInput.IsAdmin = false;
            mappedInput.CustomerId = CurrentUserId;
            var output = await _notificationDataBLL.GetPagedListAsync(mappedInput);
            return Ok(output);
        }

        [HttpGet]
        [Route("GetCountOfUnRead")]
        public async Task<IActionResult> GetCountOfUnRead()
        {
            NotificationFilterParameterDto _NotificationFilterParameterDto = new NotificationFilterParameterDto();
            _NotificationFilterParameterDto.IsAdmin = false;
            _NotificationFilterParameterDto.CustomerId = CurrentUserId;
            var output =await _notificationDataBLL.GetCountOfUnRead(_NotificationFilterParameterDto);
            return Ok(output);
        }


        [HttpGet]
        [Route("MarkAllNotificationsToBeRead")]
        public IActionResult MarkAllNotificationsToBeRead()
        {
            NotificationFilterParameterDto _NotificationFilterParameterDto = new NotificationFilterParameterDto();
            _NotificationFilterParameterDto.IsAdmin = false;
            _NotificationFilterParameterDto.CustomerId = CurrentUserId;
            var output = _notificationDataBLL.MarkAllNotificationsToBeRead(_NotificationFilterParameterDto);
            return Ok(output);
        }

        [HttpGet]
        [Route("MarkOneNotificationToBeRead")]
        public IActionResult MarkOneNotificationToBeRead(int notificationId)
        {
            NotificationFilterParameterDto _NotificationFilterParameterDto = new NotificationFilterParameterDto();
            _NotificationFilterParameterDto.IsAdmin = false;
            _NotificationFilterParameterDto.NotificationId = notificationId;
            _NotificationFilterParameterDto.CustomerId = CurrentUserId;
            var output = _notificationDataBLL.MarkOneNotificationToBeRead(_NotificationFilterParameterDto);
            return Ok(output);
        }

        [HttpGet]
        [Route("MarkNotificationToBeHide")]
        public IActionResult MarkNotificationToBeHide(int notificationId)
        {
            NotificationFilterParameterDto _NotificationFilterParameterDto = new NotificationFilterParameterDto();
            _NotificationFilterParameterDto.IsAdmin = false;
            _NotificationFilterParameterDto.NotificationId = notificationId;
            _NotificationFilterParameterDto.CustomerId = CurrentUserId;
            var output = _notificationDataBLL.MarkOneNotificationToBeHide(_NotificationFilterParameterDto);
            return Ok(output);
        }

        #endregion

        #endregion


    }
}
