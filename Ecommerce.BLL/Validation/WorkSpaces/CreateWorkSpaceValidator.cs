using FluentValidation;
using Ecommerce.DTO.WorkSpaces;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.WorkSpaces
{
    internal partial class WorkSpaceValidator
    {
        internal class CreateWorkSpaceValidator : DtoValidationAbstractBase<CreateWorkSpaceDto>
        {
            public CreateWorkSpaceValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                   ;


                When(x => !x.IsCloud, () =>

                    {
                        RuleFor(x => x.DatabaseName)
                            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                            ;
                        RuleFor(x => x.ServerIP)
                            .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                            ;

                    }
                );


                When(x => !x.IsDefault, () =>
                {
                    RuleFor(x => x.DexefCurrencyId)
                   .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                   .GreaterThan(0)
                  ;

                    RuleFor(x => x.DexefCountryId)
                       .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                       .GreaterThan(0)
                      ;
                });


            }
        }
        internal class UpdateWorkSpaceValidator : DtoValidationAbstractBase<UpdateWorkSpaceDto>
        {
            public UpdateWorkSpaceValidator()
            {

                RuleFor(x => x.Id)
                   .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                   .GreaterThan(0)
                  ;

                Include(new CreateWorkSpaceValidator());

            }
        }
    }
}
