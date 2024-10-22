using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class FileStorage:EntityBase
    {
        public FileStorage()
        {
            AddOnSliders = new HashSet<AddOnSlider>();
            AddOns = new HashSet<AddOn>();
            ApplicationSliders = new HashSet<ApplicationSlider>();
            Applications = new HashSet<Application>();
            Customers = new HashSet<Customer>();
            Employees = new HashSet<Employee>();
            Features = new HashSet<Feature>();
            LicenseFiles = new HashSet<LicenseFile>();
            Licenses = new HashSet<License>();
            ModuleSliders = new HashSet<ModuleSlider>();
            Modules = new HashSet<Module>();
            Versions = new HashSet<Version>();
            Workspaces = new HashSet<Workspace>();
        }

        public int Id { get; set; }
        public Guid Name { get; set; }
        public string RealName { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public decimal FileSize { get; set; }
        public int FileTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? EntityId { get; set; }
        public int? TableNameId { get; set; }

        public virtual Employee CreatedByNavigation { get; set; }
        public virtual FileType FileType { get; set; }
        public virtual ICollection<AddOnSlider> AddOnSliders { get; set; }
        public virtual ICollection<AddOn> AddOns { get; set; }
        public virtual ICollection<ApplicationSlider> ApplicationSliders { get; set; }
        public virtual ICollection<Application> Applications { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Feature> Features { get; set; }
        public virtual ICollection<LicenseFile> LicenseFiles { get; set; }
        public virtual ICollection<License> Licenses { get; set; }
        public virtual ICollection<ModuleSlider> ModuleSliders { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<Version> Versions { get; set; }
        public virtual ICollection<Workspace> Workspaces { get; set; }
    }
}
