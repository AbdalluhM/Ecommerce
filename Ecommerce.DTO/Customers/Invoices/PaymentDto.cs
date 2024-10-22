using Ecommerce.Core.Entities;
using static Ecommerce.DTO.Customers.CustomerDto;

namespace Ecommerce.DTO.Customers.Invoices
{
    public class PaymentDto
    {
        public Invoice Invoice { get; set; }

        public string CurrencyCode { get; set; }

        public GetCustomerOutputDto CustomerInfo { get; set; }
    }
}
