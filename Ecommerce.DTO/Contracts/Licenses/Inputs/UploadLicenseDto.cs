using Microsoft.AspNetCore.Http;

namespace Ecommerce.DTO.Contracts.Licenses.Inputs
{
    public class UploadLicenseDto
    {
        public int RequestId { get; set; }

        public int RequestTypeId { get; set; }

        public IFormFile File { get; set; }
    }
}
