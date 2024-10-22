using FluentValidation;
using Ecommerce.DTO.Customers.Review.Customers;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Review.Customers
{
    public class NewReviewDtoValidator : DtoValidationAbstractBase<NewReviewDto>
    {
        public NewReviewDtoValidator()
        {
            RuleFor(x => x.CustomerId)
              .NotEmpty(MessageCodes.Required)
              .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.ApplicationId)
                .NotEmpty(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            RuleFor(x => x.Rate)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.InvalidRate)
                .LessThanOrEqualTo(5).WithDXErrorCode(MessageCodes.InvalidRate);

            RuleFor(x => x.Review)
                .MaximumLength(2000).WithDXErrorCode(MessageCodes.InvalidMaxLength, 2000);
        }
    }
}