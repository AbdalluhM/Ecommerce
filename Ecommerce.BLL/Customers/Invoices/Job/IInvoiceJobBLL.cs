using Ecommerce.DTO.Customers.Invoices;

using System.Threading.Tasks;

namespace Ecommerce.BLL.Customers.Invoices.Job
{
    public interface IInvoiceJobBLL
    {
        Task RenewInvoicesAutoAsync();
        // for inserting CRM Old Invoices
        Task RenewOldInvoicesAutoAsync();

        //check invoices that will be expired within 3 days or today
        Task CheckInvoicesExpireationAutoAsync();

        void DeleteOldGeneratedInoivcesPdf();
        Task UpdateFawryReferenceInvoices();
        Task UpdateFawryReferenceInvoice(int invoiceId);

        Task BlockWorkSpaceAfterTrialPeriod();
    }
}
