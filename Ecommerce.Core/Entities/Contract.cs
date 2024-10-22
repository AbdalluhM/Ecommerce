using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Contract:EntityBase
    {
        public int Id { get; set; }
        public string Serial { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Customer IdNavigation { get; set; }
    }
}
