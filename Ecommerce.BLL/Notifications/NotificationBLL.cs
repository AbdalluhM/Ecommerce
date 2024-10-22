using AutoMapper;
using Dexef.Notification.Concretes.Mailing.SmtpService;
using Dexef.Notification.Enums;
using Dexef.Notification.Models.Mailing;
using Dexef.Notification.Models.Sms;
using Dexef.Notification.Resolvers;
using Dexef.System.Validations;
using Microsoft.Extensions.Options;
using Ecommerce.DTO.Settings.Notifications;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Notifications
{
    public class NotificationBLL : BaseBLL, INotificationBLL
    {
        private readonly EmailResolver _emailResolver;
        private readonly SmsResolver _smsResolver;
        private readonly NotificationConfig _notificationConfig;
        private readonly IMapper _mapper;

        public NotificationBLL(EmailResolver emailResolver,
                               SmsResolver smsResolver,
                               IOptions<NotificationConfig> notificationOptions,
                               IMapper mapper)
            : base(mapper)
        {
            _emailResolver = emailResolver;
            _smsResolver = smsResolver;
            _notificationConfig = notificationOptions.Value;
            _mapper = mapper;
        }

        // Todo-Sully: use interface and delegate to send emails.
        public async Task<OperationResult<MailResult>> SendEmailAsync(EmailProvider emailProvider, MailContent mailContent)
        {
            var smpt = new SmtpEmailService();

            return await smpt.SendEmailAsync(GetMailConfig(emailProvider), mailContent);
        }

        public async Task<OperationResult<List<SmsResult>>> SendSmsAsync(SmsProvider smsProvider, SmsContent SmsDto)
        {
            return await _smsResolver(smsProvider).SendSmsAsync(GetSmsConfig(smsProvider), SmsDto);
        }

        #region Helpers.
        private string GetMailConfig(EmailProvider emailProvider)
        {
            return emailProvider switch
            {
                EmailProvider.Smtp => JsonConvert.SerializeObject(_notificationConfig.MailConfig.SmtpConfig),
                _ => JsonConvert.SerializeObject(_notificationConfig.MailConfig.SmtpConfig),
            };
        }

        private string GetSmsConfig(SmsProvider smsProvider)
        {
            return smsProvider switch
            {
                SmsProvider.Twilio => JsonConvert.SerializeObject(_notificationConfig.SmsConfig.TwilioConfig),
                SmsProvider.Clickatell => JsonConvert.SerializeObject(_notificationConfig.SmsConfig.ClickatellConfig),
                SmsProvider.SmsMisr => JsonConvert.SerializeObject(_notificationConfig.SmsConfig.SmsMisrConfig),
                SmsProvider.SmsOtpMisr => JsonConvert.SerializeObject(_notificationConfig.SmsConfig.SmsMisrConfig),
                SmsProvider.Taqnyat => JsonConvert.SerializeObject(_notificationConfig.SmsConfig.TaqnyatConfig),
                SmsProvider.VictoryLink => JsonConvert.SerializeObject(_notificationConfig.SmsConfig.VictoryLinkConfig),
                _ => JsonConvert.SerializeObject(_notificationConfig.SmsConfig.TwilioConfig),
            };
        }
        #endregion
    }
}
