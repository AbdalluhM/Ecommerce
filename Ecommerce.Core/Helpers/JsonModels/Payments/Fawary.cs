using System;

namespace Ecommerce.Core.Helpers.JsonModels.Payments
{
    public class Fawry : PaymentBase
    {
        public bool IsFawryCard { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime? Expiration { get; set; }
        public string Token { get; set; }
        public string ThreeDSecureUrl { get; set; }
        public string CardNumber { get; set; }


        public string GeneratedMerchantNumber { get; set; }
    }
    public class FawryCardTokenResponse : PaymentBase
    {
        public string type { get; set; }
        public string referenceNumber { get; set; }
        public string merchantRefNumber { get; set; }
        public string orderAmount { get; set; }
        public string fawryFees { get; set; }
        public string orderStatus { get; set; }
        public string paymentMethod { get; set; }
        public string paymentTime { get; set; }
        public string customerMobile { get; set; }
        public string customerMail { get; set; }
        public string customerProfileId { get; set; }
        public string signature { get; set; }
        public string taxes { get; set; }
        public int statusCode { get; set; }
        public string statusDescription { get; set; }
        public bool basketPayment { get; set; }
    }
}
