using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class VerifyUpdateMobileDto
    {
        public int MobileId { get; set; }
        public int OldMobileId { get; set; }

        public string Code { get; set; }
        [JsonIgnore]
        public bool IsPrimaryMobile { get; set; } = true;
        [JsonIgnore]
        public bool FromLandingPage { get; set; } = false;
    }
}
