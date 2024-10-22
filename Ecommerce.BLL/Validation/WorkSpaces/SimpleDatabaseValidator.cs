using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.DataProcessing;
using FluentValidation;
using Ecommerce.DTO.WorkSpaces;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.WorkSpaces
{
    public class SimpleDatabaseValidator :DtoValidationAbstractBase<SimpleDatabaseDto>
    {
        public SimpleDatabaseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithDXErrorCode(MessageCodes.Required);
                ;
                RuleFor(x => x.Url)
                    .NotEmpty().WithDXErrorCode(MessageCodes.Required);
                ;
        }
    }
}
