using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class LicenseStatu:EntityBase
    {
        public LicenseStatu()
        {
            LicenseLogNewStatus = new HashSet<LicenseLog>();
            LicenseLogOldStatus = new HashSet<LicenseLog>();
            Licenses = new HashSet<License>();
        }

        public int Id { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<LicenseLog> LicenseLogNewStatus { get; set; }
        public virtual ICollection<LicenseLog> LicenseLogOldStatus { get; set; }
        public virtual ICollection<License> Licenses { get; set; }
    }
}
