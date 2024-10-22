using Dexef.Notification.Enums;
using Dexef.Notification.Models.Mailing;
using Dexef.Notification.Models.Sms;
using Dexef.System.Validations;
using Ecommerce.DTO.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Notifications
{
    public interface INotificationBLL
    {
        Task<OperationResult<MailResult>> SendEmailAsync(EmailProvider emailProvider, MailContent mailContent);

        Task<OperationResult<List<SmsResult>>> SendSmsAsync(SmsProvider smsProvider, SmsContent SmsDto);
    }
}
