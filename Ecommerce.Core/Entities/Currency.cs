using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Currency:EntityBase
    {
        public Currency()
        {
            CountryCurrencies = new HashSet<CountryCurrency>();
            Invoices = new HashSet<Invoice>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbole { get; set; }
        public bool IsActive { get; set; }
        public string Crmid { get; set; }
        public string Code { get; set; }
        public int? MultiplyFactor { get; set; }

        public virtual ICollection<CountryCurrency> CountryCurrencies { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
