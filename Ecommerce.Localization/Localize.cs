using Microsoft.Extensions.Localization;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Resources;

namespace Ecommerce.Localization
{
   
    public class Localize : ILocalize
    {

        /// <summary>
        /// Get error message based on key
        /// </summary>
        /// <param name="key">The key of message in resource file</param>
        /// <returns>The value of message</returns>
        public string GetErrorMessage( string key )
        {

            ResourceManager rm = new ResourceManager("DateStrings", typeof(Localize).Assembly);
            return rm.GetString(key);

        }



    }


}
