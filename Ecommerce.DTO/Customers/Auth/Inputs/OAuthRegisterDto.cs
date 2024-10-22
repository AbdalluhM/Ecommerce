using Ecommerce.Core.Enums.Auth;

namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class OAuthRegisterDto
    {
        public string Token { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string CountryCode { get; set; }

        public RegistrationSourceEnum SourceId { get; set; }
    }
   
}
