using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class DownloadAddOnLog:EntityBase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AddOnId { get; set; }
        public string Ipaddress { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual AddOn AddOn { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
