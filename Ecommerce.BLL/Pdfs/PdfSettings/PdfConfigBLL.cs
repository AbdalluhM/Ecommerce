using DinkToPdf;
using MyDexef.DTO.Settings.Pdfs;
using System.IO;
using System.Reflection;

namespace MyDexef.BLL.Pdfs.PdfSettings
{
    public class PdfConfigBLL : IPdfConfigBLL
    {
        public PdfConfigBLL()
        {
        }

        public GlobalSettings GetPdfGlobalSettings(IPdfSetting pdfSetting)
        {
            var globalSetting = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings
                {
                    Top = pdfSetting.GlobalSetting.MarginSetting.Top,
                    Bottom = pdfSetting.GlobalSetting.MarginSetting.Bottom,
                    Left = pdfSetting.GlobalSetting.MarginSetting.Left,
                    Right = pdfSetting.GlobalSetting.MarginSetting.Right,
                },
            };

            return globalSetting;
        }

        public ObjectSettings GetPdfObjectSettings(IPdfSetting pdfSettings, string htmlContent)
        {
            var headerSettings = pdfSettings.ObjectSetting.HeaderSetting;
            var footerSettings = pdfSettings.ObjectSetting.FooterSetting;

            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var objectSetting = new ObjectSettings
            {
                HtmlContent = htmlContent,
                PagesCount = pdfSettings.ObjectSetting.HasPagesCount,
                WebSettings = new WebSettings
                {
                    DefaultEncoding = "utf-8",
                    UserStyleSheet = Path.Combine(basePath,
                                                  pdfSettings.ObjectSetting.StyleSheetPath,
                                                  pdfSettings.ObjectSetting.StyleSheetName),
                },
                HeaderSettings = new HeaderSettings
                {
                    FontName = headerSettings.FontName,
                    FontSize = headerSettings.FontSize,
                    Line = headerSettings.HasLine,
                    Center = headerSettings.Center,
                    Left = headerSettings.Left,
                    Right = headerSettings.Right,
                    Spacing = headerSettings.Spacing,
                    HtmUrl = headerSettings.HtmUrl
                },
                FooterSettings = new FooterSettings
                {
                    FontName = footerSettings.FontName,
                    FontSize = footerSettings.FontSize,
                    Line = footerSettings.HasLine,
                    Center = footerSettings.Center,
                    Left = footerSettings.Left,
                    Right = footerSettings.Right,
                    Spacing = footerSettings.Spacing,
                    HtmUrl = footerSettings.HtmUrl
                }
            };

            return objectSetting;
        }
    }
}
