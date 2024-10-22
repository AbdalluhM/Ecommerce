using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Employee:EntityBase
    {
        public Employee()
        {
            AddOnCreatedByNavigations = new HashSet<AddOn>();
            AddOnLabelCreatedByNavigations = new HashSet<AddOnLabel>();
            AddOnLabelModifiedByNavigations = new HashSet<AddOnLabel>();
            AddOnModifiedByNavigations = new HashSet<AddOn>();
            AddOnPriceCreatedByNavigations = new HashSet<AddOnPrice>();
            AddOnPriceModifiedByNavigations = new HashSet<AddOnPrice>();
            AddOnSliderCreatedByNavigations = new HashSet<AddOnSlider>();
            AddOnSliderModifiedByNavigations = new HashSet<AddOnSlider>();
            AddOnTags = new HashSet<AddOnTag>();
            ApplicationCreatedByNavigations = new HashSet<Application>();
            ApplicationLabelCreatedByNavigations = new HashSet<ApplicationLabel>();
            ApplicationLabelModifiedByNavigations = new HashSet<ApplicationLabel>();
            ApplicationModifiedByNavigations = new HashSet<Application>();
            ApplicationSliderCreatedByNavigations = new HashSet<ApplicationSlider>();
            ApplicationSliderModifiedByNavigations = new HashSet<ApplicationSlider>();
            ApplicationTags = new HashSet<ApplicationTag>();
            CountryCurrencyCreatedByNavigations = new HashSet<CountryCurrency>();
            CountryCurrencyModifiedByNavigations = new HashSet<CountryCurrency>();
            CustomerReviews = new HashSet<CustomerReview>();
            Customers = new HashSet<Customer>();
            EmployeeCountries = new HashSet<EmployeeCountry>();
            FeatureCreatedByNavigations = new HashSet<Feature>();
            FeatureModifiedByNavigations = new HashSet<Feature>();
            FileStorages = new HashSet<FileStorage>();
            Invoices = new HashSet<Invoice>();
            LicenseFiles = new HashSet<LicenseFile>();
            LicenseLogs = new HashSet<LicenseLog>();
            ModuleCreatedByNavigations = new HashSet<Module>();
            ModuleModifiedByNavigations = new HashSet<Module>();
            ModuleSliderCreatedByNavigations = new HashSet<ModuleSlider>();
            ModuleSliderModifiedByNavigations = new HashSet<ModuleSlider>();
            ModuleTags = new HashSet<ModuleTag>();
            NotificationCreatedByEmployees = new HashSet<Notification>();
            NotificationHiddenByEmployees = new HashSet<Notification>();
            NotificationReadByEmployees = new HashSet<Notification>();
            PaymentMethodCreatedByNavigations = new HashSet<PaymentMethod>();
            PaymentMethodModifiedByNavigations = new HashSet<PaymentMethod>();
            PriceLevelCreatedByNavigations = new HashSet<PriceLevel>();
            PriceLevelModifiedByNavigations = new HashSet<PriceLevel>();
            RequestChangeDevices = new HashSet<RequestChangeDevice>();
            RoleCreatedByNavigations = new HashSet<Role>();
            RoleModifiedByNavigations = new HashSet<Role>();
            RolePageActionCreatedByNavigations = new HashSet<RolePageAction>();
            RolePageActionModifiedByNavigations = new HashSet<RolePageAction>();
            TagCreatedByNavigations = new HashSet<Tag>();
            TagModifiedByNavigations = new HashSet<Tag>();
            TaxCreatedByNavigations = new HashSet<Tax>();
            TaxModifiedByNavigations = new HashSet<Tax>();
            VersionAddonCreatedByNavigations = new HashSet<VersionAddon>();
            VersionAddonModifiedByNavigations = new HashSet<VersionAddon>();
            VersionCreatedByNavigations = new HashSet<Version>();
            VersionFeatureCreatedByNavigations = new HashSet<VersionFeature>();
            VersionFeatureModifiedByNavigations = new HashSet<VersionFeature>();
            VersionModifiedByNavigations = new HashSet<Version>();
            VersionModuleCreatedByNavigations = new HashSet<VersionModule>();
            VersionModuleModifiedByNavigations = new HashSet<VersionModule>();
            VersionPriceCreatedByNavigations = new HashSet<VersionPrice>();
            VersionPriceModifiedByNavigations = new HashSet<VersionPrice>();
            VersionReleases = new HashSet<VersionRelease>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string Mobile { get; set; }
        public int? RoleId { get; set; }
        public bool? IsAdmin { get; set; }
        public int? ImageId { get; set; }
        public bool? IsAdminForOtherCountries { get; set; }

        public virtual FileStorage Image { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<AddOn> AddOnCreatedByNavigations { get; set; }
        public virtual ICollection<AddOnLabel> AddOnLabelCreatedByNavigations { get; set; }
        public virtual ICollection<AddOnLabel> AddOnLabelModifiedByNavigations { get; set; }
        public virtual ICollection<AddOn> AddOnModifiedByNavigations { get; set; }
        public virtual ICollection<AddOnPrice> AddOnPriceCreatedByNavigations { get; set; }
        public virtual ICollection<AddOnPrice> AddOnPriceModifiedByNavigations { get; set; }
        public virtual ICollection<AddOnSlider> AddOnSliderCreatedByNavigations { get; set; }
        public virtual ICollection<AddOnSlider> AddOnSliderModifiedByNavigations { get; set; }
        public virtual ICollection<AddOnTag> AddOnTags { get; set; }
        public virtual ICollection<Application> ApplicationCreatedByNavigations { get; set; }
        public virtual ICollection<ApplicationLabel> ApplicationLabelCreatedByNavigations { get; set; }
        public virtual ICollection<ApplicationLabel> ApplicationLabelModifiedByNavigations { get; set; }
        public virtual ICollection<Application> ApplicationModifiedByNavigations { get; set; }
        public virtual ICollection<ApplicationSlider> ApplicationSliderCreatedByNavigations { get; set; }
        public virtual ICollection<ApplicationSlider> ApplicationSliderModifiedByNavigations { get; set; }
        public virtual ICollection<ApplicationTag> ApplicationTags { get; set; }
        public virtual ICollection<CountryCurrency> CountryCurrencyCreatedByNavigations { get; set; }
        public virtual ICollection<CountryCurrency> CountryCurrencyModifiedByNavigations { get; set; }
        public virtual ICollection<CustomerReview> CustomerReviews { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<EmployeeCountry> EmployeeCountries { get; set; }
        public virtual ICollection<Feature> FeatureCreatedByNavigations { get; set; }
        public virtual ICollection<Feature> FeatureModifiedByNavigations { get; set; }
        public virtual ICollection<FileStorage> FileStorages { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<LicenseFile> LicenseFiles { get; set; }
        public virtual ICollection<LicenseLog> LicenseLogs { get; set; }
        public virtual ICollection<Module> ModuleCreatedByNavigations { get; set; }
        public virtual ICollection<Module> ModuleModifiedByNavigations { get; set; }
        public virtual ICollection<ModuleSlider> ModuleSliderCreatedByNavigations { get; set; }
        public virtual ICollection<ModuleSlider> ModuleSliderModifiedByNavigations { get; set; }
        public virtual ICollection<ModuleTag> ModuleTags { get; set; }
        public virtual ICollection<Notification> NotificationCreatedByEmployees { get; set; }
        public virtual ICollection<Notification> NotificationHiddenByEmployees { get; set; }
        public virtual ICollection<Notification> NotificationReadByEmployees { get; set; }
        public virtual ICollection<PaymentMethod> PaymentMethodCreatedByNavigations { get; set; }
        public virtual ICollection<PaymentMethod> PaymentMethodModifiedByNavigations { get; set; }
        public virtual ICollection<PriceLevel> PriceLevelCreatedByNavigations { get; set; }
        public virtual ICollection<PriceLevel> PriceLevelModifiedByNavigations { get; set; }
        public virtual ICollection<RequestChangeDevice> RequestChangeDevices { get; set; }
        public virtual ICollection<Role> RoleCreatedByNavigations { get; set; }
        public virtual ICollection<Role> RoleModifiedByNavigations { get; set; }
        public virtual ICollection<RolePageAction> RolePageActionCreatedByNavigations { get; set; }
        public virtual ICollection<RolePageAction> RolePageActionModifiedByNavigations { get; set; }
        public virtual ICollection<Tag> TagCreatedByNavigations { get; set; }
        public virtual ICollection<Tag> TagModifiedByNavigations { get; set; }
        public virtual ICollection<Tax> TaxCreatedByNavigations { get; set; }
        public virtual ICollection<Tax> TaxModifiedByNavigations { get; set; }
        public virtual ICollection<VersionAddon> VersionAddonCreatedByNavigations { get; set; }
        public virtual ICollection<VersionAddon> VersionAddonModifiedByNavigations { get; set; }
        public virtual ICollection<Version> VersionCreatedByNavigations { get; set; }
        public virtual ICollection<VersionFeature> VersionFeatureCreatedByNavigations { get; set; }
        public virtual ICollection<VersionFeature> VersionFeatureModifiedByNavigations { get; set; }
        public virtual ICollection<Version> VersionModifiedByNavigations { get; set; }
        public virtual ICollection<VersionModule> VersionModuleCreatedByNavigations { get; set; }
        public virtual ICollection<VersionModule> VersionModuleModifiedByNavigations { get; set; }
        public virtual ICollection<VersionPrice> VersionPriceCreatedByNavigations { get; set; }
        public virtual ICollection<VersionPrice> VersionPriceModifiedByNavigations { get; set; }
        public virtual ICollection<VersionRelease> VersionReleases { get; set; }
    }
}
