using FluentValidation;
using Ecommerce.DTO.Customers.Wishlist;
using Ecommerce.DTO.Customers.WishlistApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Wishlist
{
    public class WihlistAddOnValidator
    {
        internal class CreateWihlistAddOnInputDtoValidator : DtoValidationAbstractBase<CreateWishlistAddOnInputDto>
        {
            public CreateWihlistAddOnInputDtoValidator()
            {

                RuleFor(a => a.AddOnId)
                  .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                  .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(a => a.CustomerId)
                  .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                  .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

               
            }

        }
        internal class DeleteWihlistAddOnInputDtoValidator : DtoValidationAbstractBase<DeleteWishlistAddOnInputDto>
        {
            public DeleteWihlistAddOnInputDtoValidator()
            {
                RuleFor(x => x.AddOnId)
                   .NotEmpty(MessageCodes.Required)
                   .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);   

            }

        }
       
    }
}
