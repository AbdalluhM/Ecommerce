using FluentValidation;
using Ecommerce.DTO.Customers.Review;
using Ecommerce.DTO.Customers.Review.Admins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Review
{
    public class CustomerReviewValidator
    {
        internal class SubmitCustomerReviewInputDtoValidator : DtoValidationAbstractBase<SubmitCustomerReviewInputDto>
        {
            public SubmitCustomerReviewInputDtoValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty(MessageCodes.Required)
                    .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero); 
               
            }

        }

        internal class DeleteCustomerReviewInputDtoValidator : DtoValidationAbstractBase<DeleteCustomerReviewInputDto>
        {
            public DeleteCustomerReviewInputDtoValidator()
            {
                RuleFor(x => x.Id)
                   .NotEmpty(MessageCodes.Required)
                   .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            }

        }
    }
}
