namespace Ecommerce.DTO.Settings.Auth
{
    public class SmsTemplate
    {
        public SmsTemplateContent TwilioTemplate { get; set; } = new();

        public SmsTemplateContent SmsMisrTemplate { get; set; } = new();

        public SmsTemplateContent SmsOtpMisrTemplate { get; set; } = new();

        public SmsTemplateContent TaqnyatTemplate { get; set; } = new();

        public SmsTemplateContent VictoryLinkTemplate { get; set; } = new();
    }
}
