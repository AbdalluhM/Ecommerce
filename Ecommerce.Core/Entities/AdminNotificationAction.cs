using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AdminNotificationAction:EntityBase
    {
        public int Id { get; set; }
        public string ActionTitle { get; set; }
        public int? CreatedBy { get; set; }
        public int? CreatedFor { get; set; }
        public string Description { get; set; }
    }
}
