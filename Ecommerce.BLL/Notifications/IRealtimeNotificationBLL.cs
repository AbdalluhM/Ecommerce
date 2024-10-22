using Ecommerce.Core.Enums.Notifications;
using Ecommerce.DTO.Notifications;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Notifications
{
    public interface IRealtimeNotificationBLL
    {
        Task SendMessageToUsersAsync(ProjectTypeEnum projectTypeEnum, UserNotificationDto userNotificationDto);
    }
}
