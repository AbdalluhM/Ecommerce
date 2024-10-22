using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class LicenseRequestFile:EntityBase
    {
        public LicenseRequestFile()
        {
            RequestActivationKeys = new HashSet<RequestActivationKey>();
            RequestChangeDevices = new HashSet<RequestChangeDevice>();
        }

        public int Id { get; set; }
        public int ActivationFileId { get; set; }
        public DateTime KeyUploadedOn { get; set; }

        public virtual FileStorage ActivationFile { get; set; }
        public virtual ICollection<RequestActivationKey> RequestActivationKeys { get; set; }
        public virtual ICollection<RequestChangeDevice> RequestChangeDevices { get; set; }
    }
}
