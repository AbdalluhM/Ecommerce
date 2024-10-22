namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class RegisterDto
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }
        
        public string TaxRegistrationNumber { get; set; }

        public int IndustryId { get; set; }
        
        public int CompanySizeId { get; set; }

        public string CompanyWebsite { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
