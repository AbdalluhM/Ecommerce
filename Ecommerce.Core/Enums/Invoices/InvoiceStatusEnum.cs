namespace Ecommerce.Core.Enums.Invoices
{
    public enum InvoiceStatusEnum
    {
        Unpaid = 1,
        Paid = 2,
        Draft = 3,
        Refunded = 4,
        Cancelled = 5,
        WaitingPaymentConfirmation = 6
    }
}
