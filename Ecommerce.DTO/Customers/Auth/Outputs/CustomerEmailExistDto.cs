using System.Security.AccessControl;

namespace Ecommerce.DTO.Customers.Auth.Outputs
{
    public class CustomerEmailExistDto
    {
        public bool IsEmailExist { get; set; } 

        public bool IsMobileVerified { get; set; }

        public int MobileId { get; set; }
    }
}
