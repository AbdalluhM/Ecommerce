using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Version:EntityBase
    {
        public Version()
        {
            VersionAddons = new HashSet<VersionAddon>();
            VersionFeatures = new HashSet<VersionFeature>();
            VersionModules = new HashSet<VersionModule>();
            VersionPrices = new HashSet<VersionPrice>();
            VersionReleases = new HashSet<VersionRelease>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int ApplicationId { get; set; }
        public int ImageId { get; set; }
        public string MainPageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsHighlightedVersion { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int DownloadCount { get; set; }
        public string ProductCrmId { get; set; }

        public virtual Application Application { get; set; }
        public virtual Employee CreatedByNavigation { get; set; }
        public virtual FileStorage Image { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual ICollection<VersionAddon> VersionAddons { get; set; }
        public virtual ICollection<VersionFeature> VersionFeatures { get; set; }
        public virtual ICollection<VersionModule> VersionModules { get; set; }
        public virtual ICollection<VersionPrice> VersionPrices { get; set; }
        public virtual ICollection<VersionRelease> VersionReleases { get; set; }
    }
}
