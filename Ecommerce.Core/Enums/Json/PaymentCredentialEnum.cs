namespace Ecommerce.Core.Enums.Json
{
    public enum PaymentCredentialEnum
    {
        ApiKey,
        SecertKey,
        BaseUrl
    }

    public enum PaymentTypesEnum
    {
        PayPal = 1,
        Fawry = 2,
        Bank = 3,
        WaitCashRefund = 4,
        Cash = 5,
        PayMob = 6,
    }

    public enum PaymentMethodEnum
    {
        WaitCashRefund = 1,
        Cash = 2,
    }
}
