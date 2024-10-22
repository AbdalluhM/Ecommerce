﻿using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class VersionFeature:EntityBase
    {
        public int Id { get; set; }
        public int VersionId { get; set; }
        public int FeatureId { get; set; }
        public string MoreDetail { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Feature Feature { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual Version Version { get; set; }
    }
}
