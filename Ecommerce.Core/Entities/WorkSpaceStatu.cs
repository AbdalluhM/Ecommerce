using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class WorkSpaceStatu:EntityBase
    {
        public WorkSpaceStatu()
        {
            Workspaces = new HashSet<Workspace>();
        }

        public int Id { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Workspace> Workspaces { get; set; }
    }
}
