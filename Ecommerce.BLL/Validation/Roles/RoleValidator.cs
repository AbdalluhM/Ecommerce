using FluentValidation;

using Microsoft.AspNetCore.Components.Forms;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Roles.RoleDto;

namespace Ecommerce.BLL.Validation.Roles
{
    internal class RoleValidator
    {
        public class CreateRoleInputDtoValidator : DtoValidationAbstractBase<CreateRoleInputDto>
        {
            public CreateRoleInputDtoValidator( )
            {
                RuleFor(x => x.Name)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
                RuleFor(x => x.RolePageAction)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                 .Must(x => x.ToList().TrueForAll(c => c.PageId > 0)).WithDXErrorCode(MessageCodes.GreaterThanZero, "Page")
                .Must(x => x.ToList().TrueForAll(c => c.ActionIds.Count() == 0 || (c.ActionIds.Count() > 0 && c.ActionIds.TrueForAll(a => a > 0))))
                .WithDXErrorCode(MessageCodes.GreaterThanZero, nameof(Action));

                //RuleForEach(x => x.RolePageAction)
                //    .ChildRules(page=>page.RuleFor(x=>x.PageId)).NotEmpty()
                //    .WithDXErrorCode(MessageCodes.Required);

                //RuleForEach(x => x.RolePageAction)
                //   .ChildRules(page => page.RuleFor(x => x.ActionIds)).NotEmpty()
                //   .WithDXErrorCode(MessageCodes.Required);
            }

        }
        public class UpdateRoleInputDtoValidator : DtoValidationAbstractBase<UpdateRoleInputDto>
        {
            public UpdateRoleInputDtoValidator( )
            {
                When(x => string.IsNullOrEmpty(x.Name), ( ) =>
                {
                    RuleFor(x => x.Name)
                   .NotEmpty().WithDXErrorCode(MessageCodes.Required);
                });

                RuleFor(x => x.RoleId)
                   .NotEmpty().WithDXErrorCode(MessageCodes.Required)
                   .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                //RuleForEach(x => x.RolePageAction)
                //    .ChildRules(page => page.RuleFor(x => x.PageId)).NotEmpty()
                //    .WithDXErrorCode(MessageCodes.Required);

                //RuleForEach(x => x.RolePageAction)
                //   .ChildRules(page => page.RuleFor(x => x.ActionIds)).NotEmpty()
                //   .WithDXErrorCode(MessageCodes.Required);
            }

        }
    }

}
