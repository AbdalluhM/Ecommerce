namespace Ecommerce.Core.Helpers.JsonModels.Payments
{
   

    public class InvoicePaymentInfoJson
    {
        public bool IsSuccess { get; set; }

        public Bank Bank { get; set; } = new();

        public Fawry Fawry { get; set; } = new();

        public Paypal Paypal { get; set; } = new();
    }

    public class InvoicePaymentResponseJson
    {
        public bool IsSuccess { get; set; }
        public BankResponse Bank { get; set; } = new();
        public FawryCardTokenResponse Fawry { get; set; } = new();
        public PaypalResponse Paypal { get; set; } = new();
    }

    public class RefundPaymentResponseDto
    {
        public bool IsSuccess { get; set; }

        public string Result { get; set; }
    }
}
