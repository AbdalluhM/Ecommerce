using Ecommerce.DTO.Settings.Pdfs.GlobalSetting;
using Ecommerce.DTO.Settings.Pdfs.ObjectSetting;

namespace Ecommerce.DTO.Settings.Pdfs
{
    public interface IPdfSetting
    {
        public PdfGlobalSetting GlobalSetting { get; set; }

        public PdfObjectSetting ObjectSetting { get; set; }
    }
}
