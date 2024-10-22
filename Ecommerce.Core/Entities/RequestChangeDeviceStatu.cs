using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class RequestChangeDeviceStatu:EntityBase
    {
        public RequestChangeDeviceStatu()
        {
            RequestChangeDevices = new HashSet<RequestChangeDevice>();
        }

        public int Id { get; set; }
        public string Status { get; set; }

        public virtual ICollection<RequestChangeDevice> RequestChangeDevices { get; set; }
    }
}
