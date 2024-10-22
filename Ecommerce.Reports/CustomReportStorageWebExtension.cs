using DevExpress.CodeParser;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Extensions;

using Ecommerce.Reports.DevexpressReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraReports.Native;

namespace Ecommerce.Reports
{

    //public class CustomReportStorageWebExtension : ReportStorageService
    //{

    //    readonly string reportDirectory;
    //    const string fileExtension = ".repx";
    //    ReportsFactory factory = new ReportsFactory();
    //    readonly string savedTemplatesDirectory;
    //    public CustomReportStorageWebExtension( /*IHostingEnvironment env */)
    //    {
    //        reportDirectory = Directory.GetCurrentDirectory();
    //        //for saving and retrieving report templates
    //        savedTemplatesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ReportTemplates");
    //        if (!Directory.Exists(savedTemplatesDirectory))
    //        {
    //            Directory.CreateDirectory(savedTemplatesDirectory);
    //        }
    //        //reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");

    //    }

       


    //    public override bool CanSetData(string url)
    //    {
    //        return base.CanSetData(url);
    //    }

    //    public override bool IsValidUrl(string url)
    //    {
    //        return base.IsValidUrl(url);
    //    }

    //    public override byte[] GetData(string url)
    //    {
    //        // Returns report layout data stored in a Report Storage using the specified URL. 
    //        // This method is called only for valid URLs after the IsValidUrl method is called.
    //        try
    //        {
    //            if (Directory.EnumerateFiles(reportDirectory).Select(Path.GetFileNameWithoutExtension).Contains(url))
    //            {
    //                return File.ReadAllBytes(Path.Combine(reportDirectory, url + fileExtension));
    //            }
    //            if (factory.Reports.ContainsKey(url))
    //            {
    //                using (MemoryStream ms = new MemoryStream())
    //                {
    //                    factory.Reports [url]().SaveLayoutToXml(ms);
    //                    return ms.ToArray();
    //                }
    //            }
    //            throw new DevExpress.XtraReports.c.Web.ClientControls.FaultException(string.Format("Could not find report '{0}'.", url));
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new DevExpress.XtraReports.Web.ClientControls.FaultException(string.Format("Could not find report '{0}'.", url));
    //        }
    //    }

    //    public override Dictionary<string, string> GetUrls()
    //    {
    //        return Directory.GetFiles(savedTemplatesDirectory, "*" + fileExtension).Select(Path.GetFileNameWithoutExtension).ToDictionary<string, string>(x => x);
    //    }

    //    public override void SetData(XtraReport report, string url)
    //    {
    //        report.SaveLayoutToXml(Path.Combine(savedTemplatesDirectory, url + fileExtension));

    //    }

    //    public override string SetNewData(XtraReport report, string defaultUrl)
    //    {
    //        SetData(report, defaultUrl);
    //        return defaultUrl;
    //    }
    //}
}
