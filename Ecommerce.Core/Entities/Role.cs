using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Role:EntityBase
    {
        public Role()
        {
            Employees = new HashSet<Employee>();
            RolePageActions = new HashSet<RolePageAction>();
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
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<RolePageAction> RolePageActions { get; set; }
    }
}
