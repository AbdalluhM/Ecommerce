﻿using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class JobQueue:EntityBase
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public string Queue { get; set; }
        public DateTime? FetchedAt { get; set; }
    }
}
