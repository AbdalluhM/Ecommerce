using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ViewApplicationVersionFeature:EntityBase
    {
        public string FeatureName { get; set; }
        public string ApplicationName { get; set; }
        public int ApplicationId { get; set; }
        public string VersionName { get; set; }
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
    }
}
