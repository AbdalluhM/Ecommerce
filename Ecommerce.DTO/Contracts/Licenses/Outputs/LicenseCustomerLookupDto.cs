using Ecommerce.DTO.Settings.Files;

namespace Ecommerce.DTO.Contracts.Licenses.Outputs
{
    public class LicenseCustomerLookupDto
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string ContractSerial { get; set; }

        public FileStorageDto Image { get; set; } = new();
    }
}
