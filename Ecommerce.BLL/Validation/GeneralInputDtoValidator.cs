using FluentValidation;

using Ecommerce.BLL.Validation;
using Ecommerce.DTO;
using Ecommerce.DTO.Features;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation
{

    internal class GetEntityInputDtoValidator : DtoValidationAbstractBase<GetEntityInputDto>
    {
        public GetEntityInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }

    }
    internal class DeleteEntityInputDtoValidator : DtoValidationAbstractBase<DeleteEntityInputDto>
    {
        public DeleteEntityInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
        }

    }
    internal class DeleteTrackedEntityInputDtoValidator : DtoValidationAbstractBase<DeleteTrackedEntityInputDto>
    {
        public DeleteTrackedEntityInputDtoValidator( )
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);
            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
        }

    }
}

