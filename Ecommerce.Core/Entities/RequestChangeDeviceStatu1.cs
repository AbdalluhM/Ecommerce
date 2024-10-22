using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class RequestChangeDeviceStatu1:EntityBase
    {
        public RequestChangeDeviceStatu1()
        {
            RequestChangeDevices = new HashSet<RequestChangeDevice>();
        }

        public int Id { get; set; }
        public string Status { get; set; }

        public virtual ICollection<RequestChangeDevice> RequestChangeDevices { get; set; }
    }
}
