using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Ecommerce.Helper.Extensions;
using Ecommerce.Reports.Enums;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace Ecommerce.Reports
{
    public class ReportManager : IReportManager
    {
        public ReportManager()
        {
        }

        #region public Methods

        public XtraReport GetReportInstance(ReportNames reportname)
        {
            XtraReport report = ReportInstance.GetXtraReport(reportname);
            return report;
        }

        public void PrintReportToPrinter(ReportNames reportName, object reportsource, short numberOfCopies, string printerName = null)
        {

            var report = GetReportInstance(reportName);
            BindingList<object> bl = new BindingList<object>();
            bl.Add(reportsource);
            report.DataSource = bl;
            report.CreateDocument();

            for (int i = 0; i < numberOfCopies; i++)
            {
                if (!string.IsNullOrEmpty(printerName))
                {
                    try
                    {
                        report.Print(printerName);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    report.Print();
                }
            }
        }

        public string ExportDataAsHTML(ReportNames reportName,  object reportSource = null)
        {
            var report = GetReportInstance(reportName);
            if (reportSource != null)
            {
                report = GetReportBindToDto(report, reportSource);
            }
            string strHtml;
            using (MemoryStream ms = new MemoryStream())
            {
                report.ExportToHtml(ms);
                ms.Position = 0;
                var sr = new StreamReader(ms);
                strHtml = sr.ReadToEnd();
            }
            return strHtml;
        }

        public string ExportDataAsPDF(ReportNames reportName, object reportSource = null)
        {
            var report = GetReportInstance(reportName);
            if (reportSource != null)
            {
                report = GetReportBindToDto(report, reportSource);
            }
            string strHtml;
            using (MemoryStream ms = new MemoryStream())
            {
                report.ExportToPdf(ms);
                ms.Position = 0;
                var sr = new StreamReader(ms);
                strHtml = sr.ReadToEnd();
            }
            return strHtml;
        }

        public byte[] ExportReportAsPDF(ReportNames reportName, object reportSource)
        {
            var report = GetReportInstance(reportName);
            if (reportSource != null)
            {
                report = GetReportBindToDto(report, reportSource);
            }
            MemoryStream ms = new MemoryStream();
            report.ExportToPdf(ms);
            return ms.ToArray();
        }

        public byte[] ExportAs(FormatType format, ReportNames reportName, object reportSource = null)
        {

            MemoryStream ms = new MemoryStream();
            var report = GetReportInstance(reportName);
            if (reportSource != null)
            {
                report = GetReportBindToDto(report, reportSource);
            }

            switch (format)
            {
                case FormatType.pdf:
                    report.ExportToPdf(ms);
                    break;
                case FormatType.docx:
                    report.ExportToDocx(ms);
                    break;
                case FormatType.xls:
                    report.ExportToXls(ms);
                    break;
                case FormatType.xlsx:
                    report.ExportToXlsx(ms);
                    break;
                case FormatType.rtf:
                    report.ExportToRtf(ms);
                    break;
                case FormatType.mht:
                    report.ExportToMht(ms);
                    break;
                case FormatType.html:
                    report.ExportToHtml(ms);
                    break;
                case FormatType.txt:
                    report.ExportToText(ms);
                    break;
                case FormatType.csv:
                    report.ExportToCsv(ms);
                    break;
                case FormatType.png:
                    report.ExportToImage(ms, new ImageExportOptions() { Format = System.Drawing.Imaging.ImageFormat.Png });
                    break;
                default:
                    break;
            }

            return ms.ToArray();
        }
        public XtraReport GetExtraReport(ReportNames reportName, object reportSource = null)
        {
            XtraReport xtraReport = new XtraReport();
            MemoryStream ms = new MemoryStream();
            var report = GetReportInstance(reportName);
            if (reportSource != null)
            {
                report = GetReportBindToDto(report, reportSource);
                return report;
            }
            return xtraReport;
        }



            /// <summary>
            /// Parse connection string to return each attribute as separate
            /// </summary>
            /// <param name="connectionString"></param>
            /// <returns>return collection of key and value</returns>
            private Hashtable ParseConnectionString(string connectionString)
        {
            Hashtable hashConnectionAttributes = new Hashtable();
            var connectionAttributes = connectionString.Split(';');
            hashConnectionAttributes["DataSource"] = connectionAttributes.FirstOrDefault(s => s.ToLower().Contains("Data Source".ToLower())).Split('=')[1];
            hashConnectionAttributes["IntialCatalog"] = connectionAttributes.FirstOrDefault(s => s.ToLower().Contains("Initial Catalog".ToLower())).Split('=')[1];
            hashConnectionAttributes["UserID"] = connectionAttributes.FirstOrDefault(s => s.ToLower().Contains("User ID".ToLower())).Split('=')[1];
            hashConnectionAttributes["Password"] = connectionAttributes.FirstOrDefault(s => s.ToLower().Contains("Password".ToLower())).Split('=')[1];

            return hashConnectionAttributes;

        }


        #endregion

        #region  Private Methods
        private XtraReport GetReportBindToDto(XtraReport report, object reportSource)
        {
            #region RTL Support
            var cultureName = Thread.CurrentThread.CurrentCulture.Name;
            bool isRightToLeft = cultureName.Contains("ar");
            report.RightToLeft = isRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            report.RightToLeftLayout = isRightToLeft ? RightToLeftLayout.Yes : RightToLeftLayout.No;
            #endregion

            BindingList<object> bl = new BindingList<object>();
            bl.Add(reportSource);
            report.DataSource = bl;

          
            report.CreateDocument();
            return report;
        }
        #endregion
    }
}

