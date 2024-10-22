using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class RequestActivationKeyStatu:EntityBase
    {
        public RequestActivationKeyStatu()
        {
            RequestActivationKeys = new HashSet<RequestActivationKey>();
        }

        public int Id { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<RequestActivationKey> RequestActivationKeys { get; set; }
    }
}
