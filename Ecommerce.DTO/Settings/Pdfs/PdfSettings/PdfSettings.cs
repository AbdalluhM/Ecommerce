using Ecommerce.DTO.Settings.Pdfs.GlobalSetting;
using Ecommerce.DTO.Settings.Pdfs.ObjectSetting;

namespace Ecommerce.DTO.Settings.Pdfs.PdfSettings
{
    public class InvoicePdfSetting : IPdfSetting
    {
        public PdfGlobalSetting GlobalSetting { get; set; } = new();

        public PdfObjectSetting ObjectSetting { get; set; } = new();
    }
}
