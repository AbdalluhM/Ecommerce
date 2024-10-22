using Ecommerce.Localization;

using System.ComponentModel;

namespace Ecommerce.DTO
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute( string resourceKey )
            : base(GetMessageFromResource(resourceKey))
        {
        
        }

        private static string GetMessageFromResource( string resourceKey )

        {
            ICustomStringLocalizer<ErrorMessage> _localizer = LocalizerProvider<ErrorMessage>.GetLocalizer();
            return  _localizer[resourceKey];
            // return string.Empty;
            // return the translation out of your .rsx files
        }
    }
}
