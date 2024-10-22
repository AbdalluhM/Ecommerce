using Ecommerce.DTO.Paging;

namespace Ecommerce.DTO.Customers.Invoices.Outputs
{
    public class FilterInvoiceDto : FilteredResultRequestDto
    {
        public int? InvoiceStatusId { get; set; }
    }
}
