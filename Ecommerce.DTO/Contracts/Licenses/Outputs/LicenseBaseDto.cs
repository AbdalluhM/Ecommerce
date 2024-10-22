namespace Ecommerce.DTO.Contracts.Licenses.Outputs
{
    public class LicenseBaseDto
    {
        public int? Id { get; set; }

        public string CustomerName { get; set; }

        public string ContractSerial { get; set; }

        public string ProductName { get; set; }
    }
}
