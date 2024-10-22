//using DevExpress.AspNetCore.Reporting.QueryBuilder.Native.Services;
//using DevExpress.AspNetCore.Reporting.QueryBuilder;
//using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
//using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
//using DevExpress.XtraReports.Web.ReportDesigner;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
//using DevExpress.AspNetCore.Reporting.ReportDesigner;




//namespace Ecommerce.API.Controllers
//{
//    [EnableCors]
//    [Route("DXXRDV")]
//    [ApiExplorerSettings(IgnoreApi =true)]
//    public class CustomWebDocumentViewerController : WebDocumentViewerController
//    {
//        public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService):base(controllerService)
//        {

//        }
        

//    }
//    [EnableCors]
//    [Route("DXXRD")]
//    [ApiExplorerSettings(IgnoreApi = true)]
//    public class CustomReportDesignerController : DevExpress.AspNetCore.Reporting.ReportDesigner.ReportDesignerController
//    {
//        public CustomReportDesignerController( IReportDesignerMvcControllerService controllerService ) : base(controllerService)
//        {
//        }
//        #region comment
//        //[HttpPost("[action]")]
//        //public IActionResult GetDesignerModel( [FromForm] string reportUrl, [FromServices] IReportDesignerClientSideModelGenerator modelGenerator )
//        //{
//        //    var model = modelGenerator.GetModel(reportUrl, null, DevExpress.AspNetCore.Reporting.ReportDesigner.ReportDesignerController.DefaultUri, WebDocumentViewerController.DefaultUri, QueryBuilderController.DefaultUri);
//        //    return DesignerModel(model);
//        //}
//        #endregion

//        [HttpPost("[action]")]
//        public ActionResult GetReportDesignerModel( [FromForm] string reportUrl )
//        {
//            string modelJsonScript =
//                new ReportDesignerClientSideModelGenerator(HttpContext.RequestServices)
//                .GetJsonModelScript(
//                    reportUrl,                 // The URL of a report that is opened in the Report Designer when the application starts.
//                    null, // Available data sources in the Report Designer that can be added to reports.
//                    "DXXRD",   // The URI path of the default controller that processes requests from the Report Designer.
//                    "DXXRDV",// The URI path of the default controller that that processes requests from the Web Document Viewer.
//                    "DXXQB"      // The URI path of the default controller that processes requests from the Query Builder.
//                );
//            return Content(modelJsonScript, "application/json");
//        }
//    }

//    [EnableCors]
//    [Route("DXXQB")]
//    [ApiExplorerSettings(IgnoreApi = true)]
//    public class CustomQueryBuilderController : QueryBuilderController
//    {
//        public CustomQueryBuilderController( IQueryBuilderMvcControllerService controllerService ) : base(controllerService)
//        {
//        }
//    }

//    [EnableCors]
//    [ApiController]
//    [Route("[controller]")]
//    [ApiExplorerSettings(IgnoreApi = true)]
//    public class ReportDesignerController : Controller//ControllerBase // Controller//
//    {
//        [HttpPost("[action]")]
//        public ActionResult GetReportDesignerModel( [FromForm] string reportUrl)
//        {
//            string modelJsonScript =
//                new ReportDesignerClientSideModelGenerator(HttpContext.RequestServices)
//                .GetJsonModelScript(
//                    reportUrl,                 // The URL of a report that is opened in the Report Designer when the application starts.
//                    null, // Available data sources in the Report Designer that can be added to reports.
//                    "DXXRD",   // The URI path of the default controller that processes requests from the Report Designer.
//                    "DXXRDV",// The URI path of the default controller that that processes requests from the Web Document Viewer.
//                    "DXXQB"      // The URI path of the default controller that processes requests from the Query Builder.
//                );
//            return Content(modelJsonScript, "application/json");
//        }

//    }
//}
