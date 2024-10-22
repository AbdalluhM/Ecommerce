using Ecommerce.DTO.Settings.Files;

namespace Ecommerce.DTO.Addons.AddonBase.Outputs
{
    public class AppVersionDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int VersionReleaseId { get; set; }
        public int SubscribtionTypeId { get; set; }
        public string Title { get; set; }

        //public string Logo { get; set; }
        public FileStorageDto Logo { get; set; }

    }
}
