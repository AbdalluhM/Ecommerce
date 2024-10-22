using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Ecommerce.BLL;
using Ecommerce.BLL.Employees;

using Nito.AsyncEx;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.API.Middlewares
{ 
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationMiddleware( RequestDelegate next )
        {
            _next = next;
        }
        public async Task Invoke( HttpContext context )
        {
            //get client preferred language
            var userLangs = context.Request.Headers [DXConstants.SupportedLanguage.RequestHeader].ToString();
            var firstLang = userLangs.Split(',').FirstOrDefault();
            //Comment this
           //firstLang = "ar";
            var culture="";
            //set allowed language
            var lang = DXConstants.SupportedLanguage.EN; //default
            switch (firstLang)
            {
                case DXConstants.SupportedLanguage.AR: //allowed
                    lang = DXConstants.SupportedLanguage.AR;
                    culture = DXConstants.SupportedLanguageCulture.AR;
                    break;
                case DXConstants.SupportedLanguageCulture.AR: //allowed
                    lang = DXConstants.SupportedLanguage.AR;
                    culture = DXConstants.SupportedLanguageCulture.AR;
                    break;

                case DXConstants.SupportedLanguage.EN: //allowed
                    lang = DXConstants.SupportedLanguage.EN;
                    culture = DXConstants.SupportedLanguageCulture.EN;
                    break;
                case DXConstants.SupportedLanguageCulture.EN: //allowed
                    lang = DXConstants.SupportedLanguage.EN;
                    culture = DXConstants.SupportedLanguageCulture.EN;
                    break;
                default:
                    //client language not supported
                    lang = DXConstants.SupportedLanguage.EN; //use our default
                    culture = DXConstants.SupportedLanguageCulture.EN;
                    break;
            }

            //switch culture
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            //save for later use
            context.Items ["CurrentEmployeeLanguage"] = lang;
            context.Items ["CurrentEmployeeCulture"] = Thread.CurrentThread.CurrentUICulture.Name;
            //context.Items ["LangId"] = lang;

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

        #region Old
        //var supportedCultures = new [] { new CultureInfo("en-US"), new CultureInfo("fr-FR") };  //1
        //var requestLocalizationOptions = new RequestLocalizationOptions  //2
        //{
        //    SupportedCultures = supportedCultures,
        //    SupportedUICultures = supportedCultures,

        //};
        //app.UseRequestLocalization(requestLocalizationOptions);  //3


        //app.Use((context, next) =>
        //{
        //    //context.Request.Scheme = "https";
        //    //context.Request.Headers.Add("proxy_set_header HTTP_X-Forwarded-Proto", "https");
        //    //get client preferred language
        //    var userLangs = context.Request.Headers[DXConstants.SupportedLanguage.RequestHeader].ToString();
        //    var firstLang = userLangs.Split(',').FirstOrDefault();
        //    //Comment this
        //    //firstLang = "ar";

        //    //set allowed language
        //    var lang = DXConstants.SupportedLanguage.EN; //default
        //    switch (firstLang)
        //    {
        //        case DXConstants.SupportedLanguage.AR: //allowed
        //            lang = firstLang;
        //            break;
        //        default:
        //            //client language not supported
        //            lang = DXConstants.SupportedLanguage.EN; //use our default
        //            break;
        //    }

        //    //switch culture
        //    Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
        //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        //    //save for later use
        //    context.Items["CurrentEmployeeLanguage"] = lang;
        //    context.Items["CurrentEmployeeCulture"] = Thread.CurrentThread.CurrentUICulture.Name;

        //    // Call the next delegate/middleware in the pipeline
        //    return next();
        //});
        //var supportedCultures = new [] { new CultureInfo("en-US"), new CultureInfo("fr-FR") };  //1
        //var requestLocalizationOptions = new RequestLocalizationOptions  //2
        //{
        //    SupportedCultures = supportedCultures,
        //    SupportedUICultures = supportedCultures,

        //};
        //app.UseRequestLocalization(requestLocalizationOptions);  //3
        #endregion

    }

}
