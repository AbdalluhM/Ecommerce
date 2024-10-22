using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Feature:EntityBase
    {
        public Feature()
        {
            VersionFeatures = new HashSet<VersionFeature>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int LogoId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual FileStorage Logo { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual ICollection<VersionFeature> VersionFeatures { get; set; }
    }
}
