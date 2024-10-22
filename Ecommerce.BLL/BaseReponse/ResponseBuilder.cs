using FluentValidation.Results;

using Microsoft.Extensions.Localization;

using Ecommerce.BLL.Validation;
using Ecommerce.Core.Enums;
using Ecommerce.Localization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Responses
{
    public class ResponseBuilder<T>
    {

        private bool isSuccess;
        private T data;
        private List<TErrorField> errors;
        private Exception exception;
        private IResponse<T> response;
        private ICustomStringLocalizer<ErrorMessage> _messageLocalizer;
        private ICustomStringLocalizer<Label> _labelLocalizer;

        internal ResponseBuilder( IResponse<T> response )
        {
            //this.data = Activator.CreateInstance<T>();//TODO:recheck this
            this.response = response;
            this.errors = new List<TErrorField>();
            this.exception = null;
            _messageLocalizer = LocalizerProvider<ErrorMessage>.GetLocalizer();
            _labelLocalizer = LocalizerProvider<Label>.GetLocalizer();
        }
        internal ResponseBuilder( IResponse<T> response, ICustomStringLocalizer<ErrorMessage> messageLocalizer, ICustomStringLocalizer<Label> labelLocalizer )
        {
            //this.data = Activator.CreateInstance<T>();//TODO:recheck this
            this.response = response;
            this.errors = new List<TErrorField>();
            this.exception = null;
            this._messageLocalizer = messageLocalizer;
            this._labelLocalizer = labelLocalizer;
        }
        private void InitializeErrorsIfNot( )
        {
            if (this.errors == null)
                this.errors = new List<TErrorField>();
        }
        internal ResponseBuilder<T> AppendError( TErrorField error )
        {
            return AppendErrors(new List<TErrorField> { error });
        }

        internal ResponseBuilder<T> AppendError( MessageCodes code, string message )
        {
            return AppendErrors(new List<TErrorField>
            {
                new TErrorField
                    {
                        FieldName = "", Code = code.StringValue(),
                         Message = !string.IsNullOrWhiteSpace(message)
                                ? _messageLocalizer [code.StringValue(),_labelLocalizer[message]]
                                : _messageLocalizer [code.StringValue()]
                        //Message = !string.IsNullOrWhiteSpace(message)
                        //        ? _messageLocalizer [code.StringValue(),  _labelLocalizer[message] != null && !string.IsNullOrWhiteSpace( _labelLocalizer[message].Value)? _labelLocalizer[message]: message]
                        //        : _messageLocalizer [code.StringValue()]

                    }
               }
            );


        }
        internal ResponseBuilder<T> AppendError( MessageCodes code, string fieldName, string message )
        {
            return AppendErrors(new List<TErrorField> { new TErrorField { FieldName = fieldName, Code = code.StringValue() ,
                  Message =_messageLocalizer[code.StringValue(),!string.IsNullOrWhiteSpace(message) ?
                $"["+_labelLocalizer[message]+"]": message ]
            } });
        }
        internal ResponseBuilder<T> AppendError( ValidationFailure error )
        {
            return AppendErrors(new List<ValidationFailure> { error });
        }
        internal ResponseBuilder<T> AppendErrors( List<TErrorField> errors )
        {
            InitializeErrorsIfNot();
            this.errors.AddRange(errors);
            return this;
        }
        internal ResponseBuilder<T> AppendErrors( List<ValidationFailure> errors )
        {
            InitializeErrorsIfNot();
            foreach (var item in errors)
            {
                this.errors.Add(new TErrorField

                {
                    FieldName = item.PropertyName,
                    Code = item.ErrorCode,
                    Message = item.ErrorMessage,
                    FieldLang = item.AttemptedValue?.ToString()
                });

            }
            return this;
        }
        internal ResponseBuilder<T> WithError( TErrorField error )
        {
            return WithErrors(new List<TErrorField> { error });
        }
        internal ResponseBuilder<T> WithError( ValidationFailure error )
        {
            return WithErrors(new List<ValidationFailure> { error });
        }
        internal ResponseBuilder<T> WithErrors( List<TErrorField> errors )
        {
            InitializeErrorsIfNot();
            this.errors.AddRange(errors);
            return this;
        }
        internal ResponseBuilder<T> WithErrors( List<ValidationFailure> errors )
        {
            InitializeErrorsIfNot();
            foreach (var item in errors)
            {
                item.PropertyName = item.PropertyName == "File.File" ? "File" : item.PropertyName;
                string localizedFieldName = _labelLocalizer [item.PropertyName];
                this.errors.Add(new TErrorField
                {
                    FieldName = item.PropertyName,
                    //FieldName = !string.IsNullOrWhiteSpace(localizedFieldName)
                    //          ? $"[" + localizedFieldName + "]"
                    //          : $"[" + item.PropertyName + "]",
                    Code = item.ErrorCode,
                    Message = string.Format(item.ErrorMessage, !string.IsNullOrWhiteSpace(localizedFieldName)
                            ? $"[" + _labelLocalizer [item.PropertyName] + "]"
                            : $"[" + item.PropertyName + "]"),
                    //for (Default,Ar) in Required Fields with  jsonmodel values
                    FieldLang = item.ErrorCode == MessageCodes.Required.StringValue() ? item.AttemptedValue?.ToString() : null
                });
                ;                
                ;

            }
            return this;
        }
        internal ResponseBuilder<T> WithData( T data )
        {
            this.data = data;
            return this;
        }
        internal ResponseBuilder<T> WithException( Exception exception )
        {
            this.exception = exception;
            this.errors.Add(new TErrorField { Message = "exMessage:" + exception.Message + "ex.InnerException:" + exception.InnerException + "ex.StackTrace:" + exception.StackTrace });
            return this;
        }

        internal bool IsSuccess { get => ((errors == null || errors.Count == 0) && exception == null) ? true : false; }
        internal IResponse<T> Build( )
        {
            isSuccess = ((errors == null || errors.Count == 0) && exception == null) ? true : false;
            response.IsSuccess = isSuccess;
            response.Errors = errors;
            response.Data = data;
            return response;

        }

    }

}

