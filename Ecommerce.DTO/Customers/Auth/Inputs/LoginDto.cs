namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class LoginDto
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
    public class LoginMobileDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
