using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class License:EntityBase
    {
        public License()
        {
            LicenseLogs = new HashSet<LicenseLog>();
            Notifications = new HashSet<Notification>();
            RequestActivationKeys = new HashSet<RequestActivationKey>();
            RequestChangeDevices = new HashSet<RequestChangeDevice>();
        }

        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string Serial { get; set; }
        public int CustomerSubscriptionId { get; set; }
        public int LicenseStatusId { get; set; }
        public DateTime? ActivateOn { get; set; }
        public DateTime? RenewalDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int? ActivationFileId { get; set; }

        public virtual FileStorage ActivationFile { get; set; }
        public virtual CustomerSubscription CustomerSubscription { get; set; }
        public virtual LicenseStatu LicenseStatus { get; set; }
        public virtual ICollection<LicenseLog> LicenseLogs { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<RequestActivationKey> RequestActivationKeys { get; set; }
        public virtual ICollection<RequestChangeDevice> RequestChangeDevices { get; set; }
    }
}
