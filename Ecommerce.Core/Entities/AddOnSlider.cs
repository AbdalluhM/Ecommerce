using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AddOnSlider:EntityBase
    {
        public int Id { get; set; }
        public int AddOnId { get; set; }
        public int MediaId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual AddOn AddOn { get; set; }
        public virtual Employee CreatedByNavigation { get; set; }
        public virtual FileStorage Media { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
    }
}
