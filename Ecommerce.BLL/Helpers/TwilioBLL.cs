using Microsoft.Extensions.Options;
using Ecommerce.Core.Entities;
using Ecommerce.DTO.Settings.Auth;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Helpers
{
    public class TwilioBLL : ITwilioBLL
    {
        private readonly AuthSetting _authSetting;

        public TwilioBLL(IOptions<AuthSetting> authSetting)
        {
            _authSetting = authSetting.Value;
        }

        public async Task SendWhatsAppVerificationCodeAsync(CustomerMobile mobile, string code, string lang = SupportedLanguage.EN)
        {
            InitializeTwilioClient();

            var contentSid = GetContentSidByLanguage(lang);
            var toPhoneNumber = GetFormattedPhoneNumber(mobile.Mobile);
            var fromPhoneNumber = GetFormattedPhoneNumber(_authSetting.Sms.WatsApp.Phone);

            try
            {
                var message = await MessageResource.CreateAsync(
                               contentSid: contentSid,
                               to: toPhoneNumber,
                               from: fromPhoneNumber,
                               contentVariables: CreateContentVariables(code),
                               messagingServiceSid: _authSetting.Sms.WatsApp.MessagingServiceSId);
            }
            catch (System.Exception)
            {

                throw;
            }

        }

        private void InitializeTwilioClient()
        {
            TwilioClient.Init(_authSetting.Sms.WatsApp.AccountSID, _authSetting.Sms.WatsApp.AuthToken);
        }

        private string GetContentSidByLanguage(string lang)
        {
            return lang == SupportedLanguage.AR
                ? _authSetting.Sms.WatsApp.ContentSIdAR
                : _authSetting.Sms.WatsApp.ContentSIdEN;
        }

        private PhoneNumber GetFormattedPhoneNumber(string mobile)
        {
            return new PhoneNumber(_authSetting.Sms.WatsApp.WhatsAppPhoneTemplate.Replace(nameof(mobile), mobile));
        }

        private string CreateContentVariables(string code)
        {
            var contentVariables = new Dictionary<string, object> { { "1", code } };
            return JsonConvert.SerializeObject(contentVariables, Formatting.Indented);
        }

    }
}
