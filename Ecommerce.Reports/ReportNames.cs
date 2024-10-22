using DevExpress.XtraReports.UI;
using Ecommerce.DevExpressReports.PredefinedReports;
using Ecommerce.Reports.Enums;
using Ecommerce.Reports.Templts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Reports
{
    internal class ReportInstance
    {
        /// <summary>   property to get instance from XtraReport by reportNames enum  </summary>
        public static XtraReport GetXtraReport(ReportNames reportName) => reportName switch
        {
            ReportNames.InvoicesList => new InvoicesList(),
            ReportNames.InvoiceReport => new InvoiceReport(),
            _ => new XtraReport()
        };
      
    }
}
