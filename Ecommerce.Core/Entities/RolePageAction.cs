using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class RolePageAction:EntityBase
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PageActionId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual PageAction PageAction { get; set; }
        public virtual Role Role { get; set; }
    }
}
