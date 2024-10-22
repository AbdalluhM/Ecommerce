using Ecommerce.Localization.Resources;

using System.Globalization;

namespace Ecommerce.Localization
{
    public class LocalizerProvider<T> where T : class
    {
        private static CustomStringLocalizer<T> Localizer { get; set; } 
        public static ICustomStringLocalizer<T> GetLocalizer( CultureInfo culture = null )
        {
            Localizer = Localizer ?? new CustomStringLocalizer<T>(culture);
            return Localizer;
          //  return new CustomStringLocalizer<T>(culture);
        }
        //public static ICustomStringLocalizer<T> GetLocalizer<T>( CultureInfo culture = null )
        //{
        //    //  return new CustomStringLocalizer<T>(culture);
        //}
    }
}
