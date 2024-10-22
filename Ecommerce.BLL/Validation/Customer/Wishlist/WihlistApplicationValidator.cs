using FluentValidation;
using Ecommerce.DTO.Customers.WishlistApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Wishlist
{
    public class WihlistApplicationValidator
    {
        internal class CreateWihlistApplicationInputDtoValidator : DtoValidationAbstractBase<CreateWishlistApplicationInputDto>
        {
            public CreateWihlistApplicationInputDtoValidator()
            {

                RuleFor(a => a.ApplicationId)
                  .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                  .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(a => a.CustomerId)
                  .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                  .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            }

        }
        internal class DeleteWihlistApplicationInputDtoValidator : DtoValidationAbstractBase<DeleteWishlistApplicationInputDto>
        {
            public DeleteWihlistApplicationInputDtoValidator()
            {
                RuleFor(x => x.ApplicationId)
                   .NotEmpty(MessageCodes.Required)
                   .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            }

        }
       
    }
}
