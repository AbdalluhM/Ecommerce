using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AggregatedCounter:EntityBase
    {
        public string Key { get; set; }
        public long Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
