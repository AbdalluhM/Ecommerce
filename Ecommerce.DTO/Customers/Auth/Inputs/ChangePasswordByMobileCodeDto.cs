using System.Security.AccessControl;

namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class ChangePasswordByMobileCodeDto
    {
        public int MobileId { get; set; }

        public string Password { get; set; }

        public string Code { get; set; }
    }
}
