namespace Ecommerce.DTO.Customers.Auth.Helpers
{
    public class ContactInfoDto
    {
        public bool IsVerified { get; }
        public string UserName { get; }

        public ContactInfoDto(bool isVerified, string userName)
        {
            IsVerified = isVerified;
            UserName = userName;
        }
    }
}
