using Ecommerce.Core.Enums.Auth;

namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class VerifyEmailDto
    {
        public string Token { get; set; }

        public string Password { get; set; }

        public bool IsVerificationLink { get; set; }
    }
}
