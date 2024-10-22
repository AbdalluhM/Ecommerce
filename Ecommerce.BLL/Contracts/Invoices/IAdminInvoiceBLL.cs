using Ecommerce.BLL.Responses;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Paging;
using Ecommerce.DTO.Pdfs.Invoices;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;
using static Ecommerce.DTO.Customers.CustomerDto;

namespace Ecommerce.BLL.Contracts.Invoices
{
    public interface IAdminInvoiceBLL
    {
        Task<IResponse<PagedResultDto<RetrieveInvoiceDto>>> GetAllInvoicesPagedlist(FilterInvoiceByCountry pagedDto);
        Task<IResponse<RetrieveInvoiceDto>> CreateSupportInvoice(PaymentDetailsInputDto inputDto,int CurrentEmployeeId);
        Task<IResponse<RetrieveInvoiceDto>> UpdateInvoice(PaymentDetailsInputDto inputDto);
        Task<IResponse<RetrieveInvoiceDto>> CreateInvoice(PaymentDetailsInputDto inputDto, int CurrentEmployeeId);
        //Task<IResponse<RetrieveInvoiceDto>> UpdateInvoice(PaymentDetailsInputDto inputDto);
        Task<IResponse<RetrieveInvoiceDto>> GetInvoiceById(int invoiceId);
        Task<IResponse<List<GetInvoicesOutputDto>>> GetAllInvoices(int employeeId);
        Task<IResponse<List<GetCustomerInvoiceOutputDto>>> GetAllActiveCustomerAsync(int employeeId);
        Task<IResponse< GetCustomerVersionOrAddOnOutputDto>> GetVersionAndAddOnAsync(GetCustomerDefaultTaxInputDto inputDto);
        Task<IResponse<List<GetVersionsubscriptionInvoiceOutputDto>>> GetCutomerVersionsAsync(GetCustomerDefaultTaxInputDto inputDto);
        Task<IResponse<List<GetAddOnCanPurshasedOutputDto>>> GetAddOnCanPurshasedAsync(int customerId, int versionSubscriptionId);
        IResponse<List<GetVersionCanPurshasedOutputDto>> GetVersionCanPurshased(int customerId);
        Task<IResponse<bool>> AssignInvoicesToCustomer(List<int> ids );
        Task<IResponse<bool>> DeleteInvoice(List<int> ids);
        Task<IResponse<bool>> CancelInvoice(CancelInvoiceInputDto inputDto);
        Task<IResponse<DownloadInvoiceFileDto>> DownloadInvoicesAsync(IEnumerable<int> inovicesIds, string lang = DXConstants.SupportedLanguage.EN);
        Task<IResponse<bool>> RefundInvoiceCashAsync(int invoiceId, int currentEmployeeId);
        Task<IResponse<bool>> PayInvoiceByAdminAsync(int invoiceId , int paidBy);
    }
}
