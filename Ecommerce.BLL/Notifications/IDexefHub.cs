using Ecommerce.DTO.Notifications;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Notifications
{
    public interface IDexefHub 
    {
        Task SendMessageToUsers(UserNotificationDto userNotificationDto);
    }
}
