using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AddOnTag:EntityBase
    {
        public int Id { get; set; }
        public int AddOnId { get; set; }
        public int TagId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public bool IsFeatured { get; set; }

        public virtual AddOn AddOn { get; set; }
        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
