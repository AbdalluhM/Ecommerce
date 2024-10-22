namespace Ecommerce.DTO.Settings.EmailTemplates
{
    public class EmailTemplateSetting
    {
        public string ApiBaseUrl { get; set; }

        public string BaseFilesPath { get; set; }

        public Folder Folder { get; set; } = new();

        public string CLientBaseUrl { get; set; }

        public ClientMethod ClientMethod { get; set; } = new();
    }
}
