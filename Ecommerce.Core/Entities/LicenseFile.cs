using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class LicenseFile:EntityBase
    {
        public LicenseFile()
        {
            RequestActivationKeys = new HashSet<RequestActivationKey>();
            RequestChangeDevices = new HashSet<RequestChangeDevice>();
        }

        public int Id { get; set; }
        public int ActivationFileId { get; set; }
        public int GeneratedBy { get; set; }
        public DateTime KeyUploadedOn { get; set; }

        public virtual FileStorage ActivationFile { get; set; }
        public virtual Employee GeneratedByNavigation { get; set; }
        public virtual ICollection<RequestActivationKey> RequestActivationKeys { get; set; }
        public virtual ICollection<RequestChangeDevice> RequestChangeDevices { get; set; }
    }
}
