using FluentValidation;
using Ecommerce.DTO.Customers.CustomerProduct;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.CustomerProduct
{
    public class NewAddOnLicenseDtoValidator : DtoValidationAbstractBase<NewAddOnLicenseDto>
    {
        public NewAddOnLicenseDtoValidator()
        {
            RuleFor(x => x.LicenseId)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
           
            RuleFor(x => x.VersionSubscriptionId)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
          
            RuleFor(x => x.AddOnSubscriptionId)
               .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }
    }
}
