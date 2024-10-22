using Microsoft.Extensions.Localization;

using Ecommerce.Localization.Helpers;

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;

namespace Ecommerce.Localization.Resources
{

    public class CustomStringLocalizer<T> : ICustomStringLocalizer<T>
    {
        private CultureInfo _currentCulture => Thread.CurrentThread.CurrentCulture;
        private ResourceManager resourceManager = new ResourceManager(typeof(T));

        public IEnumerable<LocalizedString> GetAllStrings( bool includeParentCultures )
        {
            ResourceManager rm = new ResourceManager(typeof(T));
            foreach (DictionaryEntry value in rm.GetResourceSet(_currentCulture, false, true))
            {
                yield return new LocalizedString((string)value.Key, (string)value.Value);
            }
        }

        public CustomStringLocalizer( CultureInfo culture = null )
        {
            if (culture != null)
            {
                Thread.CurrentThread.CurrentCulture = culture /*?? new CultureInfo("en")*/;
                Thread.CurrentThread.CurrentUICulture = culture /*?? new CultureInfo("en")*/;

            }

        }

        public IStringLocalizer WithCulture( CultureInfo culture )
        {
            return new CustomStringLocalizer<T>(culture);

        }

        LocalizedString IStringLocalizer.this [string name]
        {
            get
            {
                return new LocalizedString(name, GetLocalizedString(name));
            }
        }

        LocalizedString IStringLocalizer.this [string name, params object [] arguments]
        {
            get
            {
                return new LocalizedString(name, GetLocalizedString(name, arguments));
            }
        }

        public string GetLocalizedString( string name, params object [] arguments )
        {
            string result = string.Empty;
            //search in resources
            result = resourceManager.GetString(name, _currentCulture) ?? name;//to get not existing localized labels by thier name not empty string
            // search in Json

            //search in database
            if (string.IsNullOrWhiteSpace(result))
            {
                result = string.Empty;

            }
            result  = !string.IsNullOrWhiteSpace(result) && arguments != null && arguments.Length > 0
                    ? string.Format(result, arguments.ToArray().PrefSuffArray("[", "]")) 
                    : result;
            return result;
        }
    }
    

}
