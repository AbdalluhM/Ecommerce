using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class List:EntityBase
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
