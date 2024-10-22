using Ecommerce.Core.Enums.Notifications;

namespace Ecommerce.DTO.Notifications
{
    public class ConnectionDto
    {
        public string UserId { get; set; }

        public NotificationRecieverTypeEnum UserType { get; set; } 
    }
}