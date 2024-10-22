using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class DexefBranch:EntityBase
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public bool IsActive { get; set; }

        public virtual Country Country { get; set; }
    }
}
