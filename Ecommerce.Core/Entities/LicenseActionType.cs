using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class LicenseActionType:EntityBase
    {
        public LicenseActionType()
        {
            LicenseLogs = new HashSet<LicenseLog>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<LicenseLog> LicenseLogs { get; set; }
    }
}
