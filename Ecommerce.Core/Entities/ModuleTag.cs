using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ModuleTag:EntityBase
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int TagId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public bool IsFeatured { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Module Module { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
