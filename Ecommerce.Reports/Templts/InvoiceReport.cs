using DevExpress.XtraReports.UI;

using System.ComponentModel;
using System.Threading;
namespace Ecommerce.Reports.Templts
{
    public partial class InvoiceReport : DevExpress.XtraReports.UI.XtraReport
    {
        public InvoiceReport()
        {
            InitializeComponent();

        //    var parameterNumberOfRecords = new DevExpress.DataAccess.ObjectBinding.Parameter()
        //    {
        //        Name = "recordCount",
        //        Type = typeof(DevExpress.DataAccess.Expression),
        //        Value = new DevExpress.DataAccess.Expression("?NumberOfRecords", typeof(int)) ,
        //    };
        }

        public void InitReport()
        {
            //ObjectDataSource objectDataSource = new ObjectDataSource();
            //objectDataSource.DataSource = new InvoicePdfOutputDto
            //{
            //    CustomerInfo = new CustomerInfoPdfDto
            //    {
            //        Name = "abdallluh mousa",
            //    },
            //};
        }

        private void InvoiceTest_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var cultureName = Thread.CurrentThread.CurrentCulture.Name;
            this.RightToLeft = cultureName.Contains("ar") ? RightToLeft.Yes : RightToLeft.No;
            this.RightToLeftLayout = cultureName.Contains("ar") ? RightToLeftLayout.Yes : RightToLeftLayout.No;
            this.xrTable1.RightToLeft = cultureName.Contains("ar") ? RightToLeft.Yes : RightToLeft.No;
            this.InvoiceData.RightToLeft = cultureName.Contains("ar") ? RightToLeft.Yes : RightToLeft.No;
            this.GroupHeader1.RightToLeft = cultureName.Contains("ar") ? RightToLeft.Yes : RightToLeft.No;
        }
        private void InvoiceTest_BeforePrint( object sender,CancelEventArgs e )
        {
            var cultureName = Thread.CurrentThread.CurrentCulture.Name;
            this.RightToLeft = cultureName.Contains("ar") ? RightToLeft.Yes : RightToLeft.No;
            this.RightToLeftLayout = cultureName.Contains("ar") ? RightToLeftLayout.Yes : RightToLeftLayout.No;
            this.xrTable1.RightToLeft = cultureName.Contains("ar") ? RightToLeft.Yes : RightToLeft.No;
            this.InvoiceData.RightToLeft = cultureName.Contains("ar") ? RightToLeft.Yes : RightToLeft.No;
            this.GroupHeader1.RightToLeft = cultureName.Contains("ar") ? RightToLeft.Yes : RightToLeft.No;
        }
    }
}
