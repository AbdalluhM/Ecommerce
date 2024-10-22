using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class NotificationActionType:EntityBase
    {
        public NotificationActionType()
        {
            NotificationActionSubTypes = new HashSet<NotificationActionSubType>();
            NotificationActions = new HashSet<NotificationAction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<NotificationActionSubType> NotificationActionSubTypes { get; set; }
        public virtual ICollection<NotificationAction> NotificationActions { get; set; }
    }
}
