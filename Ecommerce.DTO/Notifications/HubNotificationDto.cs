using System;

namespace Ecommerce.DTO.Notifications
{
    public class HubNotificationDto
    {
        public string Id { get; set; }

        public string Sender { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime NotificationDate { get; set; }
    }
}
