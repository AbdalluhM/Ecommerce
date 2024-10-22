using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Customer:EntityBase
    {
        public Customer()
        {
            CustomerCards = new HashSet<CustomerCard>();
            CustomerEmails = new HashSet<CustomerEmail>();
            CustomerMobiles = new HashSet<CustomerMobile>();
            CustomerReviews = new HashSet<CustomerReview>();
            CustomerSubscriptions = new HashSet<CustomerSubscription>();
            DownloadVersionLogs = new HashSet<DownloadVersionLog>();
            LicenseLogs = new HashSet<LicenseLog>();
            Notifications = new HashSet<Notification>();
            RefreshTokens = new HashSet<RefreshToken>();
            Tickets = new HashSet<Ticket>();
            WishListAddOns = new HashSet<WishListAddOn>();
            WishListApplications = new HashSet<WishListApplication>();
            Workspaces = new HashSet<Workspace>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public string PostalCode { get; set; }
        public string CompanyName { get; set; }
        public string TaxRegistrationNumber { get; set; }
        public int? IndustryId { get; set; }
        public int? CompanySizeId { get; set; }
        public string CompanyWebsite { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CustomerCrmid { get; set; }
        public int? ImageId { get; set; }
        public string FullAddress { get; set; }
        public int CustomerStatusId { get; set; }
        public string CrmleadId { get; set; }
        public Guid? TempGuid { get; set; }
        public int? CreatedBy { get; set; }
        public bool? FromCrm { get; set; }
        public Guid? CrmAccountStatusId { get; set; }
        public int? SourceId { get; set; }
        public string OauthResponse { get; set; }
        public int OauthTypeId { get; set; }
        public DateTime? LastPasswordUpdate { get; set; }
        public DateTime? BirthDate { get; set; }

        public virtual CompanySize CompanySize { get; set; }
        public virtual Country Country { get; set; }
        public virtual Employee CreatedByNavigation { get; set; }
        public virtual CustomerStatu CustomerStatus { get; set; }
        public virtual FileStorage Image { get; set; }
        public virtual Industry Industry { get; set; }
        public virtual OauthType OauthType { get; set; }
        public virtual RegistrationSource Source { get; set; }
        public virtual Contract Contract { get; set; }
        public virtual ICollection<CustomerCard> CustomerCards { get; set; }
        public virtual ICollection<CustomerEmail> CustomerEmails { get; set; }
        public virtual ICollection<CustomerMobile> CustomerMobiles { get; set; }
        public virtual ICollection<CustomerReview> CustomerReviews { get; set; }
        public virtual ICollection<CustomerSubscription> CustomerSubscriptions { get; set; }
        public virtual ICollection<DownloadVersionLog> DownloadVersionLogs { get; set; }
        public virtual ICollection<LicenseLog> LicenseLogs { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<WishListAddOn> WishListAddOns { get; set; }
        public virtual ICollection<WishListApplication> WishListApplications { get; set; }
        public virtual ICollection<Workspace> Workspaces { get; set; }
    }
}
