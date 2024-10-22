using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Tax:EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public decimal Percentage { get; set; }
        public bool PriceIncludeTax { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public Guid? TempGuid { get; set; }

        public virtual Country Country { get; set; }
        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
    }
}
