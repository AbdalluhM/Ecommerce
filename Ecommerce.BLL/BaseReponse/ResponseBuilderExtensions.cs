using FluentValidation.Results;
using Ecommerce.BLL.Validation;
using Ecommerce.DTO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Responses
{
    internal static class ResponseBuilderExtensions
    {
        internal static IResponse<T> CreateResponse<T>( this ResponseBuilder<T> response )
        {
            return response.Build();
        }
        internal static IResponse<T> CreateResponse<T>( this ResponseBuilder<T> response, T data )
        {
            return response.WithData(data).Build();
        }
      
        internal static IResponse<T> CreateResponse<T>( this ResponseBuilder<T> response, List<ValidationFailure> inputValidations = null )
        {
            return response.WithErrors(inputValidations).Build();
        }
        internal static IResponse<T> CreateResponse<T, IInputDto, IValidator>( this ResponseBuilder<T> response, IInputDto dto, IValidator validator ) where IValidator : DtoValidationAbstractBase<IInputDto> where IInputDto : BaseDto

        {
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
                return response.WithErrors(validationResult.Errors).Build();
            else
                return response.Build();
        }

        //for one business error
        internal static IResponse<T> CreateResponse<T>( this ResponseBuilder<T> response, MessageCodes messageCode, string message = "" )
        {
            return response.AppendError(messageCode, message).Build();
        }
        internal static IResponse<T> CreateResponse<T>( this ResponseBuilder<T> response, Exception ex )
        {
            return response.WithException(ex).Build();
        }
    }

    //internal static class ResponseBuilderExtensions
    //{
    //    internal static ITResponse<T> CreateResponse<T>( this ResponseBuilder<T> response )
    //    {
    //        return response.Build();
    //    }
    //    internal static ITResponse<T> CreateResponse<T>( this ResponseBuilder<T> response, T data )
    //    {
    //        return response.WithData(data).Build();
    //    }
    //    internal static ITResponse<T> CreateResponse<T>( this ResponseBuilder<T> response, List<ValidationFailure> inputValidations = null )
    //    {
    //        return response.WithErrors(inputValidations).Build();
    //    }
    //    internal static ITResponse<T> CreateResponse<T, IInputDto, IValidator>( this ResponseBuilder<T> response, IInputDto dto, IValidator validator ) where IValidator : DtoValidationAbstractBase<IInputDto> where IInputDto : BaseDto

    //    {
    //        var validationResult = validator.Validate(dto);
    //        if (!validationResult.IsValid)
    //            return response.WithErrors(validationResult.Errors).Build();
    //        else
    //            return response.Build();
    //    }

    //    //for one business error
    //    internal static ITResponse<T> CreateResponse<T>( this ResponseBuilder<T> response, MessageCodes messageCode, string message = "" )
    //    {
    //        return response.AppendError(messageCode, message).Build();
    //    }
    //    internal static ITResponse<T> CreateResponse<T>( this ResponseBuilder<T> response, Exception ex )
    //    {
    //        return response.WithException(ex).Build();
    //    }
    //}

}
