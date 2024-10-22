using FluentValidation.Results;
using Ecommerce.BLL.Validation;

using System;
using System.Collections.Generic;
using System.Text;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Responses
{

    public interface IResponse<T> // where T : class
    {
        public bool IsSuccess { get; set; }
        public List<TErrorField> Errors { get; set; }
        public T Data { get; set; }
        IResponse<T> CreateResponse( );

        public IResponse<T> AppendError( TErrorField error );

        public IResponse<T> CreateResponse( T data );

        public IResponse<T> CreateResponse( List<ValidationFailure> inputValidations = null );
        //public ITResponse<T> CreateResponse<IInputDto, IValidator>( IInputDto dto, IValidator validator ) where IValidator : DtoValidationAbstractBase<IInputDto> where IInputDto : BaseDto;

        //for one business error
        public IResponse<T> CreateResponse( MessageCodes messageCode, string message = "");
        public IResponse<T> CreateResponse( Exception ex );

        public IResponse<T> AppendError( MessageCodes code, string message );
        public IResponse<T> AppendError( MessageCodes code, string fieldName, string message );
        public IResponse<T> AppendError( ValidationFailure error );
        public IResponse<T> AppendErrors( List<TErrorField> errors );
        public IResponse<T> AppendErrors( List<ValidationFailure> errors );

    }

}
