using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Action:EntityBase
    {
        public Action()
        {
            PageActions = new HashSet<PageAction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<PageAction> PageActions { get; set; }
    }
}
