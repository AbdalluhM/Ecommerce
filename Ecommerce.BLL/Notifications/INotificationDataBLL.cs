using Ecommerce.BLL.Responses;
using Ecommerce.DTO;
using Ecommerce.DTO.Notifications;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Paging;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Notifications
{
    public interface INotificationDataBLL
    {

        #region notifications operations      
        Task<IResponse<PagedResultDto<GetNotificationOutputDto>>> GetPagedListAsync(NotificationFilteredResultRequestDto pagedDto);
        Task<IResponse<int>> GetCountOfUnRead(NotificationFilterParameterDto notificationFilterParameterDto);
        IResponse<bool> MarkAllNotificationsToBeRead(NotificationFilterParameterDto notificationFilterParameterDto);
        IResponse<bool> MarkOneNotificationToBeRead(NotificationFilterParameterDto notificationFilterParameterDto);
        IResponse<bool> MarkOneNotificationToBeHide(NotificationFilterParameterDto notificationFilterParameterDto);
        Task<IResponse<int>> CreateAsync(GetNotificationForCreateDto inputDto);
        Task<IResponse<bool>> DeleteRelatedNotificationAsync(DeleteNotificationDto inputDto);
        #endregion
    }
}