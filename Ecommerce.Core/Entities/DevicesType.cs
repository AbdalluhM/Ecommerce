using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class DevicesType:EntityBase
    {
        public DevicesType()
        {
            Applications = new HashSet<Application>();
        }

        public int Id { get; set; }
        public string Device { get; set; }
        public string DeviceDescription { get; set; }

        public virtual ICollection<Application> Applications { get; set; }
    }
}
