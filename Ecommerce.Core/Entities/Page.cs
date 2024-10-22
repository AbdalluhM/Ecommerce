using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Page:EntityBase
    {
        public Page()
        {
            PageActions = new HashSet<PageAction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<PageAction> PageActions { get; set; }
    }
}
