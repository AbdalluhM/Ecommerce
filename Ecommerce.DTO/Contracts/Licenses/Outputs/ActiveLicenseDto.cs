using Ecommerce.DTO.Settings.Files;

namespace Ecommerce.DTO.Contracts.Licenses.Outputs
{
    public class ActiveLicenseDto : ExpiredLicenseDto
    {
        public FileStorageDto LicenseFile { get; set; } = new();
    }
}
