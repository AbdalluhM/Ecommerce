using FluentValidation;
using Ecommerce.BLL.Validation;
using Ecommerce.DTO.Customers.Auth.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Auth
{
    public class AppleRegisterDtoValidator: DtoValidationAbstractBase<AppleRegisterDto>
    {
        public AppleRegisterDtoValidator()
        {
            RuleFor(x => x.TokenId)
             .NotEmpty().WithDXErrorCode(MessageCodes.Required);

            RuleFor(x => x.Email)
             .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            
            RuleFor(x => x.Name)
             .NotEmpty().WithDXErrorCode(MessageCodes.Required);
            


        }
    }
}


