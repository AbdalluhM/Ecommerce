using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class RequestChangeDevice:EntityBase
    {
        public int Id { get; set; }
        public int LicenseId { get; set; }
        public string OldDeviceName { get; set; }
        public string NewDeviceName { get; set; }
        public string OldSerial { get; set; }
        public string NewSerial { get; set; }
        public int ReasonChangeDeviceId { get; set; }
        public int RequestChangeDeviceStatusId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? LicenseFileId { get; set; }

        public virtual License License { get; set; }
        public virtual LicenseFile LicenseFile { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual ReasonChangeDevice ReasonChangeDevice { get; set; }
        public virtual RequestChangeDeviceStatu RequestChangeDeviceStatus { get; set; }
    }
}
