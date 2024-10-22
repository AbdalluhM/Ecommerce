using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Module:EntityBase
    {
        public Module()
        {
            ModuleSliders = new HashSet<ModuleSlider>();
            ModuleTags = new HashSet<ModuleTag>();
            VersionModules = new HashSet<VersionModule>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int ImageId { get; set; }
        public string MainPageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual FileStorage Image { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual ICollection<ModuleSlider> ModuleSliders { get; set; }
        public virtual ICollection<ModuleTag> ModuleTags { get; set; }
        public virtual ICollection<VersionModule> VersionModules { get; set; }
    }
}
