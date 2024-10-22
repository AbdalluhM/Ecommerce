using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class NotificationActionPage:EntityBase
    {
        public NotificationActionPage()
        {
            NotificationActions = new HashSet<NotificationAction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<NotificationAction> NotificationActions { get; set; }
    }
}
