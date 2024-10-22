namespace Ecommerce.DTO.Settings.Pdfs.ObjectSetting
{
    public class PdfObjectSetting
    {
        public bool HasPagesCount { get; set; }

        public string StyleSheetPath { get; set; }

        public string StyleSheetName { get; set; }

        public PdfHeaderSetting HeaderSetting { get; set; } = new();

        public PdfFooterSetting FooterSetting { get; set; } = new();
    }
}
