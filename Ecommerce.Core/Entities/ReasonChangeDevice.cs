using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ReasonChangeDevice:EntityBase
    {
        public ReasonChangeDevice()
        {
            RequestChangeDevices = new HashSet<RequestChangeDevice>();
        }

        public int Id { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<RequestChangeDevice> RequestChangeDevices { get; set; }
    }
}
