using FluentValidation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class AppleLoginDtoValidator : DtoValidationAbstractBase<OAuthRegisterDto>
    {
        public AppleLoginDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.Mobile)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.CountryCode)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.SourceId)
                .IsInEnum().WithDXErrorCode(MessageCodes.InvalidSource);
        }
    }
}
