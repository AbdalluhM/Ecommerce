using System.ComponentModel;

namespace Ecommerce.Core.Enums.Admins
{
    public enum SubscriptionTypeEnum
    {
        [Description("Forever")]
        Forever = 1,
        [Description("Monthly")]
        Others = 2,
        [Description("All")]
        All = 3,
    }

    public enum DiscriminatorsEnum
    {
        [Description("Forever")]
        Forever = 1,
        [Description("Monthly")]
        Monthly = 2,
        [Description("Yearly")]
        Yearly = 3

    }

    public enum InvoiceTypes
    {
        [Description("Support")]
        Support = 1,
        [Description("Renewal")]
        Renewal = 2,
        [Description("Forever Subscription")]
        ForeverSubscription = 3

    }
    public enum AdminInvoiceTypesEnum
    {
        [Description("Support")]
        Support = 1,
        [Description("Product")]
        Product = 2,
      

    }
}
