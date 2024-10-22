using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Set:EntityBase
    {
        public string Key { get; set; }
        public double Score { get; set; }
        public string Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
