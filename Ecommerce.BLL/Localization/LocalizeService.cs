using Microsoft.Extensions.Localization;

using Ecommerce.Localization;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//[assembly: ResourceLocation("Resources")]
//[assembly: RootNamespace("Ecommerce.Localization")]
namespace Ecommerce.Localization
{
    public interface ILocalizeService
    {
         string? GetMessage( string key );
    }
   public class LocalizeService : ILocalizeService
    {
        private readonly IStringLocalizer<LocalizeService> _localizer = null!;

        public LocalizeService( IStringLocalizer<LocalizeService> localizer ) =>
            _localizer = localizer;

        [return: NotNullIfNotNull("_localizer")]
        public string? GetMessage( string key )
        {
            LocalizedString localizedString = _localizer [key];
            return localizedString;
        }
    }
}
