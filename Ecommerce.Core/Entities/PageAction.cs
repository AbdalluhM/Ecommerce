using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class PageAction:EntityBase
    {
        public PageAction()
        {
            RolePageActions = new HashSet<RolePageAction>();
        }

        public int Id { get; set; }
        public int PageId { get; set; }
        public int ActionId { get; set; }

        public virtual Action Action { get; set; }
        public virtual Page Page { get; set; }
        public virtual ICollection<RolePageAction> RolePageActions { get; set; }
    }
}
