using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Hash:EntityBase
    {
        public string Key { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
