using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class VersionRelease:EntityBase
    {
        public VersionRelease()
        {
            DownloadVersionLogs = new HashSet<DownloadVersionLog>();
            VersionSubscriptions = new HashSet<VersionSubscription>();
        }

        public int Id { get; set; }
        public int VersionId { get; set; }
        public string ReleaseNumber { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual Version Version { get; set; }
        public virtual ICollection<DownloadVersionLog> DownloadVersionLogs { get; set; }
        public virtual ICollection<VersionSubscription> VersionSubscriptions { get; set; }
    }
}
