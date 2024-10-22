using FluentValidation;
using Ecommerce.DTO.Contracts.Licenses.Inputs;

namespace Ecommerce.BLL.Validation.Licenses
{
    public class NewDeviceDtoValidator : DtoValidationAbstractBase<NewDeviceDto>
    {
        public NewDeviceDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithDXErrorCode(DXConstants.MessageCodes.GreaterThanZero);

            RuleFor(x => x.SubscriptionId)
                .GreaterThan(0).WithDXErrorCode(DXConstants.MessageCodes.GreaterThanZero);

            RuleFor(x => x.DeviceName)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);

            RuleFor(x => x.Serial)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);
        
            RuleFor(x => x.RenewalDate)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);
            
            RuleFor(x => x.File)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required)
                .NotEmpty().WithDXErrorCode(DXConstants.MessageCodes.Required);
        }
    }
}
