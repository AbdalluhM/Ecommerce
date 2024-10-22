using Ecommerce.Core.Enums.Customers;

namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public record ResendMobileCodeDto(int MobileId, SmsTypeEnum SmsType, bool isWhatsapp);
}
