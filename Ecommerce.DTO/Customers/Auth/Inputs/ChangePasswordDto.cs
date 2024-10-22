namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class ChangePasswordDto
    {
        public int CustomerId { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
    }
}
