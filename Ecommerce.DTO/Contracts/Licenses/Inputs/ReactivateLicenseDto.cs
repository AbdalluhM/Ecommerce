using Microsoft.AspNetCore.Http;

namespace Ecommerce.DTO.Contracts.Licenses.Inputs
{
    public class ReactivateLicenseDto
    {
        public int LicenseId { get; set; }

        public IFormFile File { get; set; }
    }
}
