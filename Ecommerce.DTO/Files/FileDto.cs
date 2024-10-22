using Microsoft.AspNetCore.Http;

namespace Ecommerce.DTO.Files
{
    public class FileDto : BaseDto, IFileDto
    {
        public IFormFile File { get; set; }

        public string FileBaseDirectory { get; set; }

        public string FilePath { get; set; }

        public string ContainerName { get; set; }
    }


}
