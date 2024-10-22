namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class ResetPasswordDto
    {
        public int CustomerId { get; set; }

        public string CurrentPassword { get; set; }

        public string Password { get; set; }
        public bool IsWatsApp { get; set; }
    }
}
