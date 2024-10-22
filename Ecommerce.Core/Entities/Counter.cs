using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Counter:EntityBase
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
