using System.Collections.Generic;

namespace Ecommerce.DTO.Settings.Notifications.Mails
{
    public class ContactUsConfig
    {
        public string DisplayName { get; set; }
        public string Subject { get; set; }
        public List<string> ToEmails { get; set; } = new();
    }
}
