namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class AlternativeMobileDto
    {
        public int CustomerId { get; set; }

        public int MobileId { get; set; }

        public string Mobile { get; set; }

        public string MobileCountryCode { get; set; }
    }
}
