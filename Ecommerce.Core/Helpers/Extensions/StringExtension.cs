using System;
using System.Text;

namespace Ecommerce.Core.Helpers.Extensions
{
    public static class StringExtension
    {
        private static readonly Random random = new();
        private const int MinLength = 10;
        private const int MaxLength = 15;
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string RemoveWhitespace(this string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        public static string GetRandomString(this StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException(nameof(sb)); // Null check for safety

            int length = random.Next(MinLength, MaxLength + 1);

            for (int i = 0; i < length; i++)
            {
                sb.Append(Chars[random.Next(Chars.Length)]);
            }

            return sb.ToString();
        }
    }
}
