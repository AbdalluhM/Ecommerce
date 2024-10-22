using Ecommerce.DTO.Settings.Notifications.Mails;
using Ecommerce.DTO.Settings.Notifications.Sms;

namespace Ecommerce.DTO.Settings.Notifications
{
    public class NotificationConfig
    {
        public MailConfig MailConfig { get; set; } = new();
        
        public SmsConfig SmsConfig { get; set; } = new();
    }
}
