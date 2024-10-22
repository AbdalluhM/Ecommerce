using Ecommerce.DTO.Settings.Files;

namespace Ecommerce.DTO.Applications.ApplicationBase.Outputs
{
    public class AppAddonInfoDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        //public string Logo { get; set; }
        public FileStorageDto Logo { get; set; }

    }
}
