using Ecommerce.BLL.Responses;
using Ecommerce.Core.Enums.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;

namespace Ecommerce.BLL.Payments
{
    public interface IPaymentSetupBLL
    {

        Task<IResponse<GetPaymentSetupOutputDto>> CreateAsync(CreatePaymentSetupInputDto inputDto);
        Task<IResponse<GetPaymentSetupOutputDto>> UpdateAsync(UpdatePaymentSetupInputDto inputDto);
        Task<IResponse<List<GetPaymentSetupOutputDto>>> GetAllAsync();
        Task<IResponse<List<GetPaymentMethodsOutputDto>>> GetAllPaymentMethodsAsync( int countryId);

        Task<IResponse<bool>> DeleteAsync (DeletePayementSetupDto inputDto);
        Task<PaymentTypeDto> GetPaymentTypeByPaymentMethod( int paymentMethodId );
        Task<GetPaymentMethodsOutputDto> GetPaymentMethodByPaymentType(PaymentTypesEnum paymentType);
        Task<IResponse<GetPaymentMethodsOutputDto>> GetPaymentMethodByPaymentTypeWithResponse(PaymentTypesEnum paymentType);
        Task<GetPaymentMethodsOutputDto> GetPaymentMethodById( int paymentMethodId );

    }
}
