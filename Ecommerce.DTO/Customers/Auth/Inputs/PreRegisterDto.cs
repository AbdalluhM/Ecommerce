using System.Security.AccessControl;

namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class PreregisterDto
    {
        public string Name { get; set; }
        
        public int CountryId { get; set; }

        public string Mobile { get; set; }

        public string MobileCountryCode { get; set; }
    }
}
