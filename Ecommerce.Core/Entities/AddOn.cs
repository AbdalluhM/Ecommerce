using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class AddOn:EntityBase
    {
        public AddOn()
        {
            AddOnLabels = new HashSet<AddOnLabel>();
            AddOnPrices = new HashSet<AddOnPrice>();
            AddOnSliders = new HashSet<AddOnSlider>();
            AddOnTags = new HashSet<AddOnTag>();
            VersionAddons = new HashSet<VersionAddon>();
            WishListAddOns = new HashSet<WishListAddOn>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int LogoId { get; set; }
        public string MainPageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string DownloadUrl { get; set; }
        public int? DownloadCount { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual FileStorage Logo { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual ICollection<AddOnLabel> AddOnLabels { get; set; }
        public virtual ICollection<AddOnPrice> AddOnPrices { get; set; }
        public virtual ICollection<AddOnSlider> AddOnSliders { get; set; }
        public virtual ICollection<AddOnTag> AddOnTags { get; set; }
        public virtual ICollection<VersionAddon> VersionAddons { get; set; }
        public virtual ICollection<WishListAddOn> WishListAddOns { get; set; }
    }
}
