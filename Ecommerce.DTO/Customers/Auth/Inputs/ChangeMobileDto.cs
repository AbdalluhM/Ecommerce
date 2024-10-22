namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class ChangeMobileDto
    {
        public int MobileId { get; set; }

        public string NewMobile { get; set; }

        public string NewMobileCountryCode { get; set; }
        public bool IsWatsApp { get; set; }
    }
}
