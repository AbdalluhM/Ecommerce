namespace Ecommerce.DTO.Settings.EmailTemplates
{
    public class Folder
    {
        public Font Font { get; set; } = new();

        public Image Image { get; set; } = new();

        public HtmlFileBase Activation { get; set; } = new();

        public HtmlFileBase ChangePassword { get; set; } = new();

        public HtmlFileBase ContactUs { get; set; } = new();

        public HtmlFileBase Invoice { get; set; } = new();
    }
}
