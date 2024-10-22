using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Notifications;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.DTO.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerTestController : ControllerBase
    {
        private readonly INotificationDataBLL _notificationDataBLL;

        public CustomerTestController(INotificationDataBLL notificationDataBLL)
        {
            _notificationDataBLL = notificationDataBLL;
        }

        [HttpPost("TestNotify")]
        public async Task<IActionResult> TestAsync()
        {
            var adminIds = new List<int>
            {
                4066
            };

            //push notification for Success Payment                     
            //var _notificationItem = new GetNotificationForCreateDto();

            //_notificationItem.IsAdminSide = true;
            //_notificationItem.IsCreatedBySystem = false;
            //_notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.SuccessPayment;
            //_notificationItem.CustomerId = 1200;

            //await _realtimeNotificationBLL.SendMessageToUsersAsync(ProjectTypeEnum.Customer, notification);

            //push new notification db
            var notificationItem = new GetNotificationForCreateDto
            {
                CustomerId = 1200,
                AdminId = 4066,
                IsAdminSide = true,
                IsCreatedBySystem = false,
                NotificationActionTypeId = (int)NotificationActionTypeEnum.KeyGenerated,
                SaveInDB = false,
                HubChannel = HubChannelEnum.RefreshTokenChannel
            };

            await _notificationDataBLL.CreateAsync(notificationItem);

            return Ok();
        }
    }
}
