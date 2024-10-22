using Ecommerce.DTO.Settings.Files;

namespace Ecommerce.DTO.Applications.ApplicationModules
{
    public class AppModuleInfoDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public FileStorageDto Logo { get; set; }

    }
}
