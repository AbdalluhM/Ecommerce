using Dexef.Notification.Models.Sms.ClickatellService;
using Dexef.Notification.Models.Sms.MisrService;
using Dexef.Notification.Models.Sms.TaqnyatService;
using Dexef.Notification.Models.Sms.VictoryLinkService;
using System.Security.AccessControl;

namespace Ecommerce.DTO.Settings.Notifications.Sms
{
    public class SmsConfig
    {
        public TwilioConfig TwilioConfig { get; set; } = new();

        public ClickatellConfig ClickatellConfig { get; set; } = new();

        public MisrConfig SmsMisrConfig { get; set; } = new();

        public TaqnyatConfig TaqnyatConfig { get; set; } = new();

        public VictoryLinkConfig VictoryLinkConfig { get; set; }
    }

}
