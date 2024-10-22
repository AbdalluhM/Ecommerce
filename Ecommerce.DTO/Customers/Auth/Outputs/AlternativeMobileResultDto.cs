namespace Ecommerce.DTO.Customers.Auth.Outputs
{
    public class AlternativeMobileResultDto : PreregisterResultDto
    {
        public string Mobile { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsVerified { get; set; }
    }
}
