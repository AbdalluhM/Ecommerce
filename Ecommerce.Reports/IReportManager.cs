using DevExpress.XtraReports.UI;
using Ecommerce.Reports.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Reports
{
    public interface IReportManager
    {

        XtraReport GetReportInstance(ReportNames reportname);
        void PrintReportToPrinter(ReportNames reportName, object reportsource, short numberOfCopies, string printerName = null);
        string ExportDataAsHTML(ReportNames reportName, object reportSource = null);
        string ExportDataAsPDF(ReportNames reportName, object reportSource = null);
        byte[] ExportReportAsPDF(ReportNames reportName, object reportSource = null);
        byte[] ExportAs(FormatType format, ReportNames reportName, object reportSource = null);
        XtraReport GetExtraReport(ReportNames reportName, object reportSource = null);
    }
}
