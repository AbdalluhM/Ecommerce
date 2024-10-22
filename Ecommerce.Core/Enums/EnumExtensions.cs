using System;
using System.ComponentModel;

namespace Ecommerce.Core.Enums
{
    public static class EnumExtensions
    {
        public static int IntValue(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }
        public static string StringValue( this Enum enumValue )
        {
            return Convert.ToInt32(enumValue).ToString();
        }
        public static string GetDescription<T>( this T enumValue )
          where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs [0]).Description;
                }
            }

            return description;
        }
    }
}

