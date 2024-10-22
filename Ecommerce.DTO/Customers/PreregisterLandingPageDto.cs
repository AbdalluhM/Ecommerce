
using System.Text.Json.Serialization;

namespace Ecommerce.DTO.Customers
{
    public class PreregisterLandingPageDto
    {
        [JsonIgnore]
        public int CustomerId { get; set; }

        public string Name { get; set; }
        
        public string Email { get; set; }
        
        public string Mobile { get; set; }
        
        public string Password { get; set; }
        
        public string CountryCode { get; set; }

        public string CompanyName { get; set; }

        public int SourceId { get; set; }
    }

    public class RegisterLandingPageOutputDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
