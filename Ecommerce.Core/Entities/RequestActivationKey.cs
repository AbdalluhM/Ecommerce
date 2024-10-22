using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class RequestActivationKey:EntityBase
    {
        public RequestActivationKey()
        {
            GeneratedNorFiles = new HashSet<GeneratedNorFile>();
        }

        public int Id { get; set; }
        public int LicenseId { get; set; }
        public int? LicenseFileId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsCreatedBySystem { get; set; }
        public int? RequestActivationKeyStatusId { get; set; }

        public virtual License License { get; set; }
        public virtual LicenseFile LicenseFile { get; set; }
        public virtual RequestActivationKeyStatu RequestActivationKeyStatus { get; set; }
        public virtual ICollection<GeneratedNorFile> GeneratedNorFiles { get; set; }
    }
}
