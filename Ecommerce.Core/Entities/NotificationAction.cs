using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class NotificationAction:EntityBase
    {
        public NotificationAction()
        {
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public int? NotificationActionTypeId { get; set; }
        public bool IsAdminSide { get; set; }
        public bool IsCreatedBySystem { get; set; }
        public int? NotificationActionSubTypeId { get; set; }
        public int? NotificationActionPageId { get; set; }

        public virtual NotificationActionPage NotificationActionPage { get; set; }
        public virtual NotificationActionSubType NotificationActionSubType { get; set; }
        public virtual NotificationActionType NotificationActionType { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
