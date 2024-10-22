using Ecommerce.Core.Enums.Invoices;

using System.Collections.Generic;

namespace Ecommerce.Core.Helpers.JsonModels.Payments
{
    public class Bank : PaymentBase
    {
        public string CardNumber { get; set; }
        public int CardTypeId { get; set; }// = (int)CardTypesEnum.Visa;
        public string Template { get; set; }
        public List<string> Parameters { get; set; } = new();
        public string FortId { get; set; }//for Refund Purpose
        public string GeneratedMerchantNumber { get; set; }


    }

    public class BankResponse : PaymentBase
    {

    }
}
