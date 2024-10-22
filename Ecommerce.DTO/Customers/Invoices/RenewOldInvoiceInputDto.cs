namespace Ecommerce.DTO.Customers.Invoices
{
    public class RenewOldInvoiceInputDto
    {

        public bool SendNotifications { get; set; } = false;
        public bool SendEmails { get; set; } = false;

        public bool AutoBill { get; set; } = false;
    }
}
