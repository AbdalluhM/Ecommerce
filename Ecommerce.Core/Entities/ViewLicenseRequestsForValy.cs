using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ViewLicenseRequestsForValy:EntityBase
    {
        public int RequestId { get; set; }
        public string Type { get; set; }
        public string Serial { get; set; }
        public string ApplicationName { get; set; }
        public string VersionName { get; set; }
        public string AddonName { get; set; }
        public int? PeriodInDays { get; set; }
    }
}
