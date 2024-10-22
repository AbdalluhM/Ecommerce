using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class DownloadVersionLog:EntityBase
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int VersionIdReleaseId { get; set; }
        public string Ipaddress { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual VersionRelease VersionIdRelease { get; set; }
    }
}
