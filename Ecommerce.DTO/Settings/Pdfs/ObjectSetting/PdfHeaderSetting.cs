namespace Ecommerce.DTO.Settings.Pdfs.ObjectSetting
{
    public class PdfHeaderSetting
    {
        public int? FontSize { get; set; }
        public string FontName { get; set; }
        public string Left { get; set; }
        public string Center { get; set; }
        public string Right { get; set; }
        public bool? HasLine { get; set; }
        public double? Spacing { get; set; }
        public string HtmUrl { get; set; }
    }
}
