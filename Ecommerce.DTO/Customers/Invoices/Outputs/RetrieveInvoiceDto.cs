using System;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.DTO.Lookups;
using Ecommerce.DTO.Lookups.PriceLevels.Outputs;
using Newtonsoft.Json;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;
using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.DTO.Customers.Invoices.Outputs
{
    public class RetrieveInvoiceDto
    {
        
        public int Id { get; set; }

        public string Serial { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? StartDate { get; set; } 

        public DateTime? EndDate { get; set; } 

        public DateTime PaymentDate { get; set; }

        public string ProductTitle { get; set; }

        public string InvoiceTitle { get; set; }

        public string PaymentMethodId { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentTypeId { get; set; }

        public string PaymentType { get; set; }
        public string InvoiceStatusId { get; set; }

        public string InvoiceStatus { get; set; }


        public InvoicePaymentInfoJson PaymentInfo { get; set; } = new();

        public string Notes { get; set; }
        
        public decimal VatPercentage { get; set; }

        public decimal TotalVatAmount { get; set; }

        public decimal Discount { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }

        public string Currency { get; set; }

        public int InvoiceTypeId { get; set; }
        
        public string InvoiceType { get; set; }

        public GetSubscriptionTypeOutputDto Subscription { get; set; }

        public CustomerInfoDto Customer { get; set; } = new();

        public RetrievePriceLevelDto PriceLevel { get; set; }

        public DateTime ExpirationDate { get; set; }

        public GetCustomerSubscriptionOutputDto CustomerSubscription { get; set; }

        //[JsonIgnore]
        //public GetCustomerOutputDto CustomerData { get; set; } = new GetCustomerOutputDto();
        public int Discriminator { get; set; } = 0;

        public bool IsAddOn { get; set; }

        public int RenwalEvery { get; set; }

        public GetVersionsubscriptionInvoiceOutputDto VersionSubscription { get; set; }
        public GetAddOnSubscriptionOutputDto AddOnSubscription { get; set; }

        public string CancelReason { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string ProductTitleEn => JsonConvert.DeserializeObject<JsonLanguageModel>(ProductTitle).Default;


    }



    //TODO:Get data from db
    public class CustomerInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string CompanyName { get; set; } 
        public string Account { get; set; } 
        public string TaxReg { get; set; }
        public string Address { get; set; }
    }

    public class FawryAPICreateTokenInputDto
    {
        public int InvoiceId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public int CustomerId { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class FawryCreateTokenInputDto
    {
        public int InvoiceId { get; set; }
        public GetSimplifiedCustomerOutputDto Customer { get; set; } = new();
        public string ReturnUrl { get; set; }
    }
    public class FawryAPIDeleteTokenInputDto
    {
        public string CardToken { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public int CustomerId { get; set; }
    }
}
