
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

using HandlebarsDotNet;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Ecommerce.Core.Enums;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.Helper.String;
using Ecommerce.Localization;
using Ecommerce.Localization.Helpers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Linq;
using System.Linq.Expressions;

using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.Validation
{
    public static class FluentExtensions
    {

        private static ICustomStringLocalizer<ErrorMessage> _messageLocalizer;
        private static ICustomStringLocalizer<Label> _labelLocalizer;

        public static IRuleBuilderOptions<T, TProperty> WithDXErrorCode<T, TProperty>( this IRuleBuilderOptions<T, TProperty> ruleBuilder, MessageCodes messageCode, string label = "" )
        {



            ruleBuilder.Configure(cfg =>
            {
                _messageLocalizer = LocalizerProvider<ErrorMessage>.GetLocalizer();
                _labelLocalizer = LocalizerProvider<Label>.GetLocalizer();

                    //cfg.CascadeMode = CascadeMode.Stop;
                    var trimedPropertyName = cfg.PropertyName.Trim()?.Replace(" ", "");
                var localizedLabel = _labelLocalizer [trimedPropertyName];
                var localizedPropertyDisplayName = !string.IsNullOrWhiteSpace(localizedLabel) ? localizedLabel : trimedPropertyName;
                string localizedMessage = _messageLocalizer [messageCode.StringValue(), localizedPropertyDisplayName];
                localizedMessage = string.IsNullOrWhiteSpace(label) ? localizedMessage : _messageLocalizer [messageCode.StringValue(), label];
                cfg.Current.ErrorCode = messageCode.StringValue();
                cfg.Current.SetErrorMessage(localizedMessage);
            });

            return ruleBuilder;

        }
        public static IRuleBuilderOptions<T, TProperty> WithDXErrorCode<T, TProperty>( this IRuleBuilderOptions<T, TProperty> ruleBuilder, MessageCodes messageCode, params object [] labels )
        {




            ruleBuilder.Configure(cfg =>
            {
                _messageLocalizer = LocalizerProvider<ErrorMessage>.GetLocalizer();
                _labelLocalizer = LocalizerProvider<Label>.GetLocalizer();
                    // cfg.CascadeMode = CascadeMode.Stop;
                    var trimedPropertyName = cfg.PropertyName.Trim()?.Replace(" ", "");
                var localizedLabel = _labelLocalizer [trimedPropertyName];
                var localizedPropertyDisplayName = !string.IsNullOrWhiteSpace(localizedLabel) ? localizedLabel : trimedPropertyName;
                string localizedMessage = _messageLocalizer [messageCode.StringValue()];

                //add field name dynamically which being validated and update in localization resx file
                if (labels != null && labels.Count() > 0)
                {
                    Array.Resize(ref labels, labels.Length + 1);
                    labels [labels.Length - 1] = trimedPropertyName;
                    labels = labels.PrefSuffArray("[", "]");
                }

                localizedMessage = labels == null || labels.Count() == 0 ? localizedMessage : string.Format(localizedMessage, labels);
                cfg.Current.ErrorCode = messageCode.StringValue();
                cfg.Current.SetErrorMessage(localizedMessage);
            });

            return ruleBuilder;
        }
        public static IRuleBuilderOptions<T, TProperty> NotEmpty<T, TProperty>( this IRuleBuilder<T, TProperty> ruleBuilder, MessageCodes messageCode, string message = "" )
        {
            return ruleBuilder.NotEmpty().WithDXErrorCode(messageCode, message);

        }
        public static IRuleBuilderOptions<T, TProperty> ValidJson2<T, TProperty, TPropertyType>( this IRuleBuilderInitial<T, TProperty> ruleBuilder, Expression<Func<TPropertyType, bool>> exp )
        {

            return ruleBuilder.SetValidator(new JsonValidator<T, TProperty, TPropertyType>(exp));

        }
        public static IRuleBuilderOptions<T, TProperty> ValidJson2<T, TProperty, TPropertyType>(this IRuleBuilderOptions<T, TProperty> ruleBuilder, Expression<Func<TPropertyType, bool>> exp)
        {

            return ruleBuilder.SetValidator(new JsonValidator<T, TProperty, TPropertyType>(exp));

        }
        public static IRuleBuilderOptionsConditions<T, TProperty> ValidJson<T, TProperty>( this IRuleBuilderInitial<T, TProperty> ruleBuilder, bool isHtml = false )
        {
            return ruleBuilder.Custom(( x, context ) =>
             {
                 _messageLocalizer = LocalizerProvider<ErrorMessage>.GetLocalizer();
                 _labelLocalizer = LocalizerProvider<Label>.GetLocalizer();

                 var name = getJsonLanguageOrNull(x + "");
                 //  var instanceToValidate = context.InstanceToValidate;
                 if (string.IsNullOrWhiteSpace(x + ""))
                 {
                     context.AddFailure(new ValidationFailure(context.DisplayName, _messageLocalizer [MessageCodes.Required.StringValue()])

                     {
                         ErrorCode = MessageCodes.Required.StringValue()
                     });
                 }
                 else if (name == null)
                 {
                     context.AddFailure(new ValidationFailure(context.DisplayName, _messageLocalizer [MessageCodes.FailedToDeserialize.StringValue()])
                     {
                         ErrorCode = MessageCodes.FailedToDeserialize.StringValue(),

                     });
                 }
                 else
                 {
                     if (isHtml)
                     {
                         name.Default = name.Default.StripHTML();
                         name.Ar = name.Ar.StripHTML();
                     }

                     if (name == null || string.IsNullOrWhiteSpace(name.Default))
                         context.AddFailure(new ValidationFailure(context.DisplayName.Trim().Replace(" ", ""), string.Format(
                              _messageLocalizer [MessageCodes.MissingDefaultValue.StringValue()], context.DisplayName.Trim().Replace(" ", "")), nameof(JsonLanguageModel.Default))
                         {
                             ErrorCode = MessageCodes.Required.StringValue(),
                         });
                     if (name == null || string.IsNullOrWhiteSpace(name.Ar))
                         context.AddFailure(new ValidationFailure(context.DisplayName.Trim().Replace(" ", ""), string.Format(
                             _messageLocalizer [MessageCodes.MissingArabicValue.StringValue()], context.DisplayName.Trim().Replace(" ", "")), nameof(JsonLanguageModel.Ar))
                         {
                             ErrorCode = MessageCodes.Required.StringValue(),
                         });

                 }

             });

        }


        public static IRuleBuilderOptionsConditions<T, TProperty> ValidJson<T, TProperty>( this IRuleBuilderInitial<T, TProperty> ruleBuilder, Expression<Func<JsonLanguageModel, bool>> exp, MessageCodes messageCode = MessageCodes.InvalidJson, string message = "" )
        {
            return ruleBuilder.Custom(( x, context ) =>
            {
                _messageLocalizer = LocalizerProvider<ErrorMessage>.GetLocalizer();
                _labelLocalizer = LocalizerProvider<Label>.GetLocalizer();

                var name = getJsonLanguageOrNull(x + "");

                //  var instanceToValidate = context.InstanceToValidate;
                if (string.IsNullOrWhiteSpace(x + ""))
                {
                    context.AddFailure(new ValidationFailure(context.DisplayName, _messageLocalizer [MessageCodes.Required.StringValue()])
                    {
                        ErrorCode = MessageCodes.Required.StringValue(),
                    }
                    );

                }


                else if (name == null)
                {
                    context.AddFailure(new ValidationFailure(context.DisplayName, _messageLocalizer [MessageCodes.FailedToDeserialize.StringValue()])
                    {
                        ErrorCode = MessageCodes.FailedToDeserialize.StringValue(),

                    });
                }
                else
                {
                    //execute expression
                    if (!exp.Compile().Invoke(name))
                    {
                        context.AddFailure(new ValidationFailure(context.DisplayName, string.IsNullOrWhiteSpace(message)
                            ? _messageLocalizer [MessageCodes.InvalidJson.StringValue()]
                            : message)

                        {
                            ErrorCode = MessageCodes.InvalidJson.StringValue(),

                        });
                    }

                    if (string.IsNullOrWhiteSpace(name.Default) || string.IsNullOrWhiteSpace(name.Ar))
                        context.AddFailure(new ValidationFailure(context.DisplayName, string.Format(_messageLocalizer [MessageCodes.Required.StringValue()], context.DisplayName))
                        {
                            ErrorCode = MessageCodes.Required.StringValue(),

                        });
                }

            });

        }

        private static bool isValidJsonLanguage( string jsonString )
        {
            try
            {
                var name = JsonConvert.DeserializeObject<JsonLanguageModel>(jsonString);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }
        private static bool isValidJsonLanguage<T>( string jsonString )
        {
            try
            {
                var name = JsonConvert.DeserializeObject<T>(jsonString);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }
        private static JsonLanguageModel getJsonLanguageOrNull( string jsonString )
        {
            try
            {
                var name = JsonConvert.DeserializeObject<JsonLanguageModel>(jsonString);
                return name;
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        #region Custom Validator
        public class JsonValidator<T, TProperty, TPropertyType> : PropertyValidator<T, TProperty>
        {

            public override string Name => "JsonValidator";
            private Expression<Func<TPropertyType, bool>> exp;
            public JsonValidator( Expression<Func<TPropertyType, bool>> exp )
            {
                this.exp = exp;
            }
            public override bool IsValid( ValidationContext<T> context, TProperty value )
            {
                if (string.IsNullOrWhiteSpace(value + ""))
                    return false;
                var canDeserialize = isValidJsonLanguage<TPropertyType>(value + "");
                if (!canDeserialize)
                    return false;
                var property = JsonConvert.DeserializeObject<TPropertyType>(value + "");
                //expresson
                if (!exp.Compile().Invoke(property))
                    return false;

                return true;
            }

            protected override string GetDefaultMessageTemplate( string errorCode )
              => "'{PropertyName}' must be valid Json.";
        }

        #endregion
    }


}
