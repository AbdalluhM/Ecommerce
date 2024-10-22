using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class JobParameter:EntityBase
    {
        public long JobId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual Job Job { get; set; }
    }
}
