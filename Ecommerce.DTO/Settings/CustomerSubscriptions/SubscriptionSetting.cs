using System;

namespace Ecommerce.DTO.Settings.CustomerSubscriptions
{
    public class SubscriptionSetting
    {
        public TimeSpan CancellationPeriod { get; set; }

        public TimeSpan ForeverRenewEvery { get; set; }
        
        public TimeSpan AnnualRenewEvery { get; set; }
        
        public TimeSpan MonthlyRenewEvery { get; set; }
        
        public string InvoiceSerialPrefix { get; set; }
        
        public string ContractSerialPrefix { get; set; }
    }
}
