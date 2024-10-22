namespace Ecommerce.Core.Helpers.JsonModels.Payments
{
    public class Paypal : PaymentBase
    {
        public string Account { get; set; }
        public string ApproveOrderLink { get; set; }
        public string UpdateOrderLink { get; set; }
        public string CaptureOrderLink { get; set; }
        public string GetOrderLink { get; set; }
        public string CaptureId { get; set; }//for Refund Purpose


    }
    public class PaypalResponse : PaymentBase
    {

    }
    public class PayMobResponse : PaymentBase
    {
        public int Amount { get; set; }
        public int MerchantRefNumber { get; set; }

    }
}
