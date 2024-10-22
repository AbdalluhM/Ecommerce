namespace Ecommerce.DTO.Settings.Files
{
    public class AdminFileBase
    {
        public string Path { get; set; }

        public string ContainerName { get; set; }
    }

    public class Addon : AdminFileBase
    {
        public string AddonBase { get; set; }

        public string AddonSlider { get; set; }
    }

    public class Modules : AdminFileBase
    {
        public string Base { get; set; }
        public string ModuleSlider { get; set; }
    }

    public class Applications : AdminFileBase
    {
        public string Base { get; set; }
        public string ApplicationSlider { get; set; }
        public string ApplicationVersion { get; set; }
    }
}
