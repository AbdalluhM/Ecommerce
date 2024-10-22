using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.FluentExtensions;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;

namespace Ecommerce.BLL.Validation.PaymentSetup
{
    public class PaymentSetupValidator
    {
        public class CreatePaymentSetupInputDtoValidator : DtoValidationAbstractBase<CreatePaymentSetupInputDto>
        {
            public CreatePaymentSetupInputDtoValidator()
            {

                RuleFor(x => x.Name)
                    .NotEmpty(MessageCodes.Required)
                    ;
                RuleFor(x => x.PaymentTypeId)
                      .NotEmpty(MessageCodes.Required)
                      .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                RuleFor(x => x.PaymentSetupCredential.ApiKey)
                    .NotEmpty(MessageCodes.Required);

                RuleFor(x => x.PaymentSetupCredential.SercretKey)
                    .NotEmpty(MessageCodes.Required);

                RuleFor(x => x.PaymentSetupCredential.BaseUrl)
                    .NotEmpty(MessageCodes.Required);



            }
           
        }
        public class UpdatePaymentSetupInputDtoValidator : DtoValidationAbstractBase<UpdatePaymentSetupInputDto>
        {
            public UpdatePaymentSetupInputDtoValidator()
            {

               

                RuleFor(x => x.Id)
                     .NotEmpty(MessageCodes.Required)
                     .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

               
                    RuleFor(x => x.PaymentTypeId)
                      .NotEmpty(MessageCodes.Required)
                      .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);

                    //RuleFor(x => x.PaymentSetupCredential.ApiKey)
                    //    .NotEmpty(MessageCodes.Required);

                    //RuleFor(x => x.PaymentSetupCredential.SercretKey)
                    //    .NotEmpty(MessageCodes.Required);

                    //RuleFor(x => x.PaymentSetupCredential.BaseUrl)
                    //    .NotEmpty(MessageCodes.Required);

            }

        }
        public class DeletePaymentSetupInputDtoValidator : DtoValidationAbstractBase<DeletePayementSetupDto>
        {
            public DeletePaymentSetupInputDtoValidator()
            {
                RuleFor(x => x.Id)
                      .NotEmpty(MessageCodes.Required)
                      .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


            }

        }
        public class InputPayementSetupDtoValidator : DtoValidationAbstractBase<InputPayementSetupDto>
        {
            public InputPayementSetupDtoValidator()
            {
                RuleFor(x => x.Id)
                      .NotEmpty(MessageCodes.Required)
                      .GreaterThan(0).WithDXErrorCode(MessageCodes.GreaterThanZero);


            }

        }
    }
}
