using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ApplicationTag:EntityBase
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int TagId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public bool IsFeatured { get; set; }

        public virtual Application Application { get; set; }
        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
