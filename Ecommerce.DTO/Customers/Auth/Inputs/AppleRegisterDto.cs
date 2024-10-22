using Ecommerce.Core.Enums.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class AppleRegisterDto
    {
        public string TokenId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Mobile { get; set; }

        public string CountryCode { get; set; }

        public RegistrationSourceEnum SourceId { get; set; }
    }
}


