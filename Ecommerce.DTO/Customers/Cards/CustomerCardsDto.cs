using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Customers.Cards
{
   

    public class CardTokenDto
    {

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int PaymentMethodId { get; set; }
        public string CardNumber { get; set; }
        public string CardToken { get; set; }
        public string ExtraInfo { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDefault { get; set; }
        public string CallBackUrl { get; set; }
    }

    public class CreateCardTokenInputDto
    {
        [JsonIgnore]
        public int CustomerId { get; set; }
        public int PaymentMethodId { get; set; }
        public string ReturnUrl { get; set; }

        //public string CardNumber { get; set; }
        //public string CardToken { get; set; }
        //public string ExtraInfo { get; set; }
        //public DateTime CreateDate { get; set; }
        //public DateTime? ModifiedDate { get; set; }
        //public bool IsDefault { get; set; }
    }

    public class UpdateCardTokenDto
    {
        public int Id { get; set; }
        public bool IsDefault { get; set; }

    }
    public class UpdateCardTypeInputDto
    {
        public int CustomerId { get; set; }
        public int PaymentMethodId { get; set; }

        public string CardToken { get; set; }
        public string CardType { get; set; }
        public bool Commit { get; set; } = false;

    }
    public class CreateCardTokenCallBackDto
    {
        public int CustomerId { get; set; }
        public int PaymentMethodId { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string Token { get; set; }
        public long CreationDate { get; set; }
        public DateTime CreationDateTime { get; }
        public string FirstSixDigits { get; set; }
        public string LastFourDigits { get; set; }
        //public string Brand { get; set; }
        //public bool IsDefault { get; set; }
    }
    public class DeleteCardTokenInputDto
    {
        public int CustomerId { get; set; }
        public int PaymentMethodId { get; set; }
        public string Token { get; set; }

    }
    public class FawryCardDto : CardTokenDto
    {
        public string Token { get; set; }
        public long CreationDate { get; set; }
        public DateTime CreationDateTime { get; }
        public string LastFourDigits { get; set; }
        public string Brand { get; set; }
    }
}
