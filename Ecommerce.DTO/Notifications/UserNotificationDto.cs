using Ecommerce.Core.Enums.Notifications;
using System.Collections.Generic;

namespace Ecommerce.DTO.Notifications
{
    public class UserNotificationDto
    {
        public List<int> Recievers { get; set; } = new();

        public NotificationRecieverTypeEnum RecieverType { get; set; }

        public HubChannelEnum Channel { get; set; }

        public GetNotificationOutputDto Notification { get; set; } = new();
    }
}
