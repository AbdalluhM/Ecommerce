using FluentValidation;

using Ecommerce.BLL.Validation.Files;
using Ecommerce.DTO.Customers.Invoices.Outputs;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.Customer.Payments
{

    internal class APIPayAndSubscribeInputDtoValidator : DtoValidationAbstractBase<APIPayAndSubscribeInputDto>
    {
        public APIPayAndSubscribeInputDtoValidator( )
        {
            When(x => !x.IsAddOn, ( ) =>
            {
                RuleFor(x => x.VersionReleaseId)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.VersionPriceId)
                  .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                  .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.CustomerId)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            });
            When(x => x.IsAddOn, ( ) =>
            {
                RuleFor(x => x.AddonPriceId)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.VersionSubscriptionId)
                  .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                  .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.CustomerId)
             .NotEmpty().WithDXErrorCode(MessageCodes.Required)
             .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            });

        }

    }
}
