using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class GeneratedNorFile:EntityBase
    {
        public int Id { get; set; }
        public int RequestActivationKeyId { get; set; }
        public byte[] NorFile { get; set; }
        public bool IsGrabbed { get; set; }

        public virtual RequestActivationKey RequestActivationKey { get; set; }
    }
}
