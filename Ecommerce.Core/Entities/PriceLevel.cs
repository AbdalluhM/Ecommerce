using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class PriceLevel:EntityBase
    {
        public PriceLevel()
        {
            AddOnPrices = new HashSet<AddOnPrice>();
            VersionPrices = new HashSet<VersionPrice>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfLicenses { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual ICollection<AddOnPrice> AddOnPrices { get; set; }
        public virtual ICollection<VersionPrice> VersionPrices { get; set; }
    }
}
