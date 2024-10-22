using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class LicensesStatu:EntityBase
    {
        public LicensesStatu()
        {
            Licens = new HashSet<Licens>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Licens> Licens { get; set; }
    }
}
