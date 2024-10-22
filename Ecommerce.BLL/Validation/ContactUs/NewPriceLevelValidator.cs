using FluentValidation;
using Ecommerce.DTO.ContactUs;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation.ContactUs
{
    public class NewContactRequestDtoValidator : DtoValidationAbstractBase<NewContactRequestDto>
    {
        public NewContactRequestDtoValidator()
        {
            RuleFor(r => r.HelpMessage)
                    .NotEmpty(MessageCodes.Required);

            RuleFor(r => r.FromEmail)
                .EmailAddress().WithDXErrorCode(MessageCodes.InvalidEmail)
                .NotEmpty(MessageCodes.Required);
            
            RuleFor(r => r.FullName)
                    .NotEmpty(MessageCodes.Required);        
            
            RuleFor(r => r.Message)
                    .NotEmpty(MessageCodes.Required);        
            
            RuleFor(r => r.CompanyName)
                    .NotEmpty(MessageCodes.Required);

            RuleFor(r => r.CompanySize)
                    .NotEmpty(MessageCodes.Required);
            
            RuleFor(r => r.MobileNumber)
                    .NotEmpty(MessageCodes.Required);        
            
            RuleFor(r => r.Industry)
                    .NotEmpty(MessageCodes.Required);
            
            RuleFor(r => r.Country)
                    .NotEmpty(MessageCodes.Required);
        }
    }
}
