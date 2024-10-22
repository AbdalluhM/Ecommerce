using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Customers.CustomerProduct.CustomerProductDto;

namespace Ecommerce.BLL.Validation.Customer.CustomerProduct
{
    public class CustomerProductValidator
    {
        internal class CreateLicenseInputDtoValidator : DtoValidationAbstractBase<CreateLicenseInputDto>
        {
            public CreateLicenseInputDtoValidator()
            {
                RuleFor(x => x.DeviceName)
                   .NotEmpty(MessageCodes.Required)
                  ;
                RuleFor(x => x.SerialNumber)
                .NotEmpty(MessageCodes.Required)
               ;
            }

        }
        internal class UpdateLicenseInputDtoValidator : DtoValidationAbstractBase<UpdateLicenseInputDto>
        {
            public UpdateLicenseInputDtoValidator()
            {
                RuleFor(x => x.DeviceId)
                    .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
                RuleFor(x => x.DeviceName)
                   .NotEmpty(MessageCodes.Required)
                  ;
                RuleFor(x => x.SerialNumber)
                  .NotEmpty(MessageCodes.Required)
                 ;
            }

        }
    }
}
