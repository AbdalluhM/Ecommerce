using DevExpress.XtraReports.UI;

using Ecommerce.DevExpressReports.PredefinedReports;
using Ecommerce.Reports.Templts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Reports.DevexpressReports
{
  public class ReportsFactory
    {
        public Dictionary<string , Func<XtraReport>> Reports{ get
            {
                return new Dictionary<string, Func<XtraReport>>()
                {
                     
                    ["InvoiceReport"] =()=> new InvoiceReport(),
                    ["Products"] = ( ) => new InvoicesList()


                };
            }
        }
    }
}
