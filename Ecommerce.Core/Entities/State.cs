﻿using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class State:EntityBase
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Data { get; set; }

        public virtual Job Job { get; set; }
    }
}
