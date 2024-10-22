using DinkToPdf;
using MyDexef.DTO.Settings.Pdfs;

namespace MyDexef.BLL.Pdfs.PdfSettings
{
    public interface IPdfConfigBLL
    {
        GlobalSettings GetPdfGlobalSettings(IPdfSetting pdfSetting);

        ObjectSettings GetPdfObjectSettings(IPdfSetting pdfSettings, string htmlContent);
    }
}
