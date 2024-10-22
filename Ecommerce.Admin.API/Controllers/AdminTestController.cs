using Microsoft.AspNetCore.Mvc;
using Ecommerce.BLL.Notifications;
using Ecommerce.Core.Enums.Notifications;
using Ecommerce.DTO.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminTestController : ControllerBase
    {
        private readonly INotificationDataBLL _notificationDataBLL;

        public AdminTestController(INotificationDataBLL notificationDataBLL)
        {
            _notificationDataBLL = notificationDataBLL;
        }

        [HttpPost("TestNotify")]
        public async Task<IActionResult> TestAsync()
        {
            var customerIds = new List<int>
            {
                1200
            };

            //var notification = new UserNotificationDto
            //{
            //    RecieverType = NotificationUserTypeEnum.Customer,
            //    Recievers = customerIds,
            //    Channel = HubChannelEnum.Dexef,
            //    Notification = new GetNotificationOutputDto
            //    {
            //        CustomerName = "Swilam Admin"
            //    }
            //};

            //await _notificationDataBLL.CreateAsync(new GetNotificationForCreateDto
            //{

            //})

            //var _notificationItem = new GetNotificationForCreateDto();
            //_notificationItem.CustomerId = request.License.CustomerSubscription.CustomerId;
            //_notificationItem.AdminId = currentEmployeeId;
            //_notificationItem.IsAdminSide = false;
            //_notificationItem.IsCreatedBySystem = false;
            //_notificationItem.NotificationActionTypeId = (int)NotificationActionTypeEnum.KeyGenerated;
            //_notificationItem.LicenceId = request.LicenseId;

            //await _notificationDataBLL.CreateAsync(_notificationItem);

            var notificationItem = new GetNotificationForCreateDto
            {
                CustomerId = 1200,
                AdminId = 4066,
                IsAdminSide = false,
                IsCreatedBySystem = false,
                NotificationActionTypeId = (int)NotificationActionTypeEnum.SubscribtionExpired,
                SaveInDB = false,
                HubChannel = HubChannelEnum.RefreshTokenChannel,
                ProjectTypeEnum = ProjectTypeEnum.Admin
            };

            await _notificationDataBLL.CreateAsync(notificationItem);

            return Ok();
        }
    }
}
