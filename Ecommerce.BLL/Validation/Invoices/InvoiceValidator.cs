using FluentValidation;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Contracts.Invoices.InvoiceDto;

namespace Ecommerce.BLL.Validation.Invoices
{
    public class InvoiceValidator
    {
        internal class CreateInvoiceInputDtoValidator : DtoValidationAbstractBase<PaymentDetailsInputDto>
        {
            public CreateInvoiceInputDtoValidator()
            {
                When(x =>string.IsNullOrEmpty(x.InvoiceTitle), () =>
                {
                    RuleFor(x => x.InvoiceTitle)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required);
                });


                RuleFor(x => x.Price)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThanOrEqualTo(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.PriceLevelId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.Discriminator)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.EndDate)
                    .GreaterThan(x=>x.StartDate).WithDXErrorCode(MessageCodes.DatePeriod);

                RuleFor(x => x.VersionId)
                 .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .GreaterThan(valueToCompare: 0).WithDXErrorCode(MessageCodes.GreaterThanZero);


                RuleFor(x => x.VersionReleaseId)
                       .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                       .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.CustomerId)
              .NotEmpty().WithDXErrorCode(MessageCodes.Required)
              .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

            }

        }
    }
}
