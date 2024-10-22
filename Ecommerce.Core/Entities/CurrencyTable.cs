using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class CurrencyTable:EntityBase
    {
        public CurrencyTable()
        {
            WorkspaceDexefCountries = new HashSet<Workspace>();
            WorkspaceDexefCurrencies = new HashSet<Workspace>();
        }

        public int Id { get; set; }
        public string ArNameCountry { get; set; }
        public string ArName { get; set; }
        public string ArNameSubName { get; set; }
        public string Shortcut { get; set; }
        public string EnName { get; set; }
        public string EnNameCountry { get; set; }
        public string EnNameSubName { get; set; }
        public string CountryCode { get; set; }
        public string DiallingCode { get; set; }

        public virtual ICollection<Workspace> WorkspaceDexefCountries { get; set; }
        public virtual ICollection<Workspace> WorkspaceDexefCurrencies { get; set; }
    }
}
