using Ecommerce.Core.Enums.Notifications;
using Ecommerce.DTO.Paging;
using System;

namespace Ecommerce.DTO.Notifications
{
    public class NotificationActionDto : BaseDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? NotificationActionTypeId { get; set; }
        public bool IsAdminSide { get; set; }
        public bool IsCreatedBySystem { get; set; }
        public string PageDetailId { get; set; }
        public int? NotificationActionSubTypeId { get; set; }
    }

    public class GetNotificationOutputDto : BaseDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public DateTime CreateDate { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        public int? ReadByEmployeeId { get; set; }
        public int? HiddenByEmployeeId { get; set; }
        public bool IsRead { get; set; }
        public bool IsHide { get; set; }
        public string CustomerName { get; set; }
        public string ActionTypeName { get; set; }
        public string Description { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? HiddenAt { get; set; }
        public string ReadByEmployeeUserName { get; set; }
        public string HiddenByEmployeeUserName { get; set; }
        public string CreatedByEmployeeUserName { get; set; }
        public int? PageDetailId { get; set; }
        public int? InvoiceId { get; set; }
        public int? LicenceId { get; set; }
        public bool IsAddOn { get; set; }
        public string TicketId { get; set; }
        public int VersionSubscribtionId { get; set; }
        public int AddonSubscribtionId { get; set; }
    }

    public class NotificationFilteredResultRequestDto : FilteredResultRequestDto
    {
        public bool IsAdmin { get; set; }
        public int? AdminId { get; set; }
        public int? CustomerId { get; set; }
    }

    public class NotificationFilterParameterDto
    {
        public bool IsAdmin { get; set; }
        public int? AdminId { get; set; }
        public int? CustomerId { get; set; }
        public int? NotificationId { get; set; }
    }

    public class GetNotificationForCreateDto : BaseDto
    {
        public int? CustomerId { get; set; }

        public int? AdminId { get; set; }
        
        public string PageDetailId { get; set; }
        
        public bool IsAdminSide { get; set; }
        
        public bool IsCreatedBySystem { get; set; }
        
        public int? NotificationActionSubTypeId { get; set; }
        
        public int? NotificationActionTypeId { get; set; }
        
        public int? InvoiceId { get; set; }
        
        public int? LicenceId { get; set; }
        
        public string TicketId { get; set; }
        
        public string TicketRefrences { get; set; }
        
        public int? RefundRequestId { get; set; }

        /// <summary>
        /// The Server that notification will be sent from. Default: Customer Server.
        /// </summary>
        public ProjectTypeEnum ProjectTypeEnum { get; set; } = ProjectTypeEnum.Customer;

        /// <summary>
        /// Save notification in database. Default: true.
        /// </summary>
        public bool SaveInDB { get; set; } = true;

        /// <summary>
        /// Hub channel to push notification on. Default: NotificationChannel.
        /// </summary>
        public HubChannelEnum HubChannel { get; set; } = HubChannelEnum.NotificationChannel;
    }


    public class DeleteNotificationDto : BaseDto
    {
        public int? InvoiceId { get; set; }
        public int? LicenceId { get; set; }
        public string TicketId { get; set; }
        public int? CustomerSubscriptionId { get; set; }
        public int? RefundRequestId { get; set; }
        

    }
}