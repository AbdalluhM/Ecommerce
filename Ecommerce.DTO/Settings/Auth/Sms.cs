using System;

namespace Ecommerce.DTO.Settings.Auth
{
    public class Sms
    {
        public TimeSpan CodeExpiryTime { get; set; }

        public TimeSpan BlockSendingTime { get; set; }

        public int SendCountLimit { get; set; }

        public WatsApp WatsApp { get; set; } = new();
        public SmsTemplate SmsTemplate { get; set; } = new();
    }

    public class WatsApp
    {
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string Phone { get; set; }
        public string ContentSIdEN { get; set; }
        public string ContentSIdAR { get; set; }
        public string MessagingServiceSId { get; set; }
        public string WhatsAppPhoneTemplate { get; set; }

    }
}
