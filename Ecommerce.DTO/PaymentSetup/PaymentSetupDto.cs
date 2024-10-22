using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.PaymentSetup
{
    public class PaymentSetupDto
    {
        public class PaymentInputDto
        {
            public string Name { get; set; }
            public int PaymentTypeId { get; set; }
            [JsonIgnore]
            public string Credential { get; set; }
            public bool IsActive { get; set; }
            public bool IsDefault { get; set; }
            public bool CalcPaymentFee { get; set; }
            public int? CountryId { get; set; }
            public PaymentSetupCredential PaymentSetupCredential { get; set; }
        }
        public class CreatePaymentSetupInputDto: PaymentInputDto
        {
            [JsonIgnore]
            public DateTime CreateDate { get; set; }
            [JsonIgnore]
            public int? CreatedBy { get; set; }
       
        }
        public class UpdatePaymentSetupInputDto : PaymentInputDto
        {
            public int Id { get; set; }
            [JsonIgnore]
            public int ModifiedBy { get; set; }
            [JsonIgnore]
            public DateTime? ModifiedDate { get; set; }
        }
        public class DeletePayementSetupDto
        {
            public int Id { get; set; }
        }
        public class InputPayementSetupDto
        {
            public int Id { get; set; }
        }
        public class PaymentSetupCredential
        {
            public string ApiKey { get; set; }
            public string SercretKey { get; set; }
            public string BaseUrl { get; set; }
        }
        #region Output
        public class GetPaymentSetupOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public PaymentTypeDto PaymentType { get; set; }
            [JsonIgnore]
            public string Credential { get; set; }
            public bool IsActive { get; set; }
            public bool IsDefault { get; set; }
            public bool CalcPaymentFee { get; set; }
            public PaymentSetupCredential PaymentSetupCredential { get; set; }
            public CountryPaymentDto CountryPayment { get; set; }
    }
        public class GetPaymentMethodsOutputDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public PaymentTypeDto PaymentType { get; set; }
            public bool IsDefault { get; set; }
            public CountryPaymentDto CountryPayment { get; set; }
            public PaymentSetupCredential Credential { get; set; }
            
        }

        public class PaymentTypeDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class PaymentMethodDto
        {
            public int Id { get; set; }
        }

        public class CountryPaymentDto
        {
            public int Id { get; set; }
            public string Country { get; set; }
        }
        #endregion
    }


}
