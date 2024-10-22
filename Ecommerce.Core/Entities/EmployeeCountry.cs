using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class EmployeeCountry:EntityBase
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int CountryCurrencyId { get; set; }

        public virtual CountryCurrency CountryCurrency { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
