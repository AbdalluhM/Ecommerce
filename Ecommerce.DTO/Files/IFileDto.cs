using Microsoft.AspNetCore.Http;

namespace Ecommerce.DTO.Files
{
    public interface IFileDto
    {
        public IFormFile File { get; set; }

        public string FileBaseDirectory { get; set; }

        public string FilePath { get; set; }

        public string ContainerName { get; set; }
    }

    public class SingleFilebaseDto
    {
        public IFileDto FileDto { get; set; }
    }
}
