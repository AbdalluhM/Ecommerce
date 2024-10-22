namespace Ecommerce.DTO.Customers.Auth.Outputs
{
    public class AlternativeEmailResultDto : PreregisterResultDto
    {
        public string Email { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsVerified { get; set; }
    }
}
