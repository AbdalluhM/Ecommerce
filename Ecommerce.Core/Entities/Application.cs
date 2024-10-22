using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Application:EntityBase
    {
        public Application()
        {
            ApplicationLabels = new HashSet<ApplicationLabel>();
            ApplicationSliders = new HashSet<ApplicationSlider>();
            ApplicationTags = new HashSet<ApplicationTag>();
            CustomerReviews = new HashSet<CustomerReview>();
            Versions = new HashSet<Version>();
            WishListApplications = new HashSet<WishListApplication>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int ImageId { get; set; }
        public int SubscriptionTypeId { get; set; }
        public string MainPageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? DeviceTypeId { get; set; }
        public bool IsService { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual DevicesType DeviceType { get; set; }
        public virtual FileStorage Image { get; set; }
        public virtual Employee ModifiedByNavigation { get; set; }
        public virtual SubscriptionType SubscriptionType { get; set; }
        public virtual ICollection<ApplicationLabel> ApplicationLabels { get; set; }
        public virtual ICollection<ApplicationSlider> ApplicationSliders { get; set; }
        public virtual ICollection<ApplicationTag> ApplicationTags { get; set; }
        public virtual ICollection<CustomerReview> CustomerReviews { get; set; }
        public virtual ICollection<Version> Versions { get; set; }
        public virtual ICollection<WishListApplication> WishListApplications { get; set; }
    }
}
