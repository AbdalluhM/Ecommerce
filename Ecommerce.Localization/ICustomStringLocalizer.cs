using Microsoft.Extensions.Localization;

namespace Ecommerce.Localization
{
    public interface ICustomStringLocalizer<T> : IStringLocalizer
    {
        public string GetLocalizedString( string name, params object [] arguments );
    }
}
