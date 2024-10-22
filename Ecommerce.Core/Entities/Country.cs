using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Country:EntityBase
    {
        public Country()
        {
            CountryPaymentMethods = new HashSet<CountryPaymentMethod>();
            CustomerMobiles = new HashSet<CustomerMobile>();
            Customers = new HashSet<Customer>();
            DexefBranches = new HashSet<DexefBranch>();
            Taxes = new HashSet<Tax>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string PhoneCode { get; set; }
        public string Crmid { get; set; }

        public virtual CountryCurrency CountryCurrency { get; set; }
        public virtual ICollection<CountryPaymentMethod> CountryPaymentMethods { get; set; }
        public virtual ICollection<CustomerMobile> CustomerMobiles { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<DexefBranch> DexefBranches { get; set; }
        public virtual ICollection<Tax> Taxes { get; set; }
    }
}
