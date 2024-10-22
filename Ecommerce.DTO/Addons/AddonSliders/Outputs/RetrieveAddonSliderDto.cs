using Ecommerce.DTO.Settings.Files;

namespace Ecommerce.DTO.Addons.AddonSliders.Outputs
{
    public class RetrieveAddonSliderDto
    {
        public int Id { get; set; }

        public FileStorageBlobDto Image { get; set; }
        public string ImagePath => Image.FullPath;

        public string Name { get; set; }

        public decimal Size { get; set; }
    }
}
