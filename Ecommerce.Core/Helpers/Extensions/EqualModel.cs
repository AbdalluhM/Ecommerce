using Ecommerce.Core.Enums.Json;

namespace Ecommerce.Core.Helpers.Extensions
{
    public class EqualModel
    {
        public EqualModel(string PropertyName, string PropertyValue, JsonKeyEnum JsonKey)
        {
            this.PropertyName = PropertyName;
            this.PropertyValue = PropertyValue;
            this.JsonKey = JsonKey;
        }

        public string PropertyName { get; }
        public string PropertyValue { get; }
        public JsonKeyEnum JsonKey { get; }
    }
}
