using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Tag:EntityBase
    {
        public Tag()
        {
            AddOnTags = new HashSet<AddOnTag>();
            ApplicationTags = new HashSet<ApplicationTag>();
            ModuleTags = new HashSet<ModuleTag>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual ICollection<AddOnTag> AddOnTags { get; set; }
        public virtual ICollection<ApplicationTag> ApplicationTags { get; set; }
        public virtual ICollection<ModuleTag> ModuleTags { get; set; }
    }
}
