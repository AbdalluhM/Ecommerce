using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class NotificationActionSubType:EntityBase
    {
        public NotificationActionSubType()
        {
            NotificationActions = new HashSet<NotificationAction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? NotificationActionTypeId { get; set; }

        public virtual NotificationActionType NotificationActionType { get; set; }
        public virtual ICollection<NotificationAction> NotificationActions { get; set; }
    }
}
