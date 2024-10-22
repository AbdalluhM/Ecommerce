using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ecommerce.Helper.String
{
    public static class StringExtentions
    {
        public static string StripHTML( this string input )
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
        public static string [] PrefixSuffixArray( this string [] arr, string prefix = "", string suffix = "" )
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr [i] = arr [i].Replace(arr [i], prefix + arr [i] + suffix);
            }
            return arr;
        }
        public static string [] PrefixSuffixArray( this object [] arr, string prefix = "", string suffix = "" )
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr [i] = arr [i].ToString().Replace(arr [i].ToString(), prefix + arr [i] + suffix);
            }
            return (string [])arr;
        }

        public static string RemoveRedundant(this string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
                return string.Empty;

            List<string> uniques = inputString.Split(',').Reverse().Distinct().Reverse().ToList();
            var output = string.Join(",", uniques);

            //remove last index of ","
            if (!string.IsNullOrWhiteSpace(output) && output.EndsWith(","))
                output = output.Remove(output.Length - 1, 1);

            return output;
        }
        public static string ConvertIntToGUIDString(this int value )
        {
            var guid = Guid.NewGuid().ToString();
            List<string> uniques = guid.Split('-').Reverse().Distinct().Reverse().ToList();
            uniques [0] = value + "";
            return string.Join("-", uniques);
        }
        public static string GetIntFromGUIDString(this string value )
        {
            List<string> uniques = value.Split('-').Reverse().Distinct().Reverse().ToList();
            return uniques [0];
        }

        public static IDictionary<string, string> GetQueryParameters(this string queryString )
        {
            var retval = new Dictionary<string, string>();
            foreach (var item in queryString.TrimStart('?').Split(new [] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var split = item.Split('=');
                retval.Add(split [0], split [1]);
            }
            return retval;
        }
        public static string JoinUriSegments(this  string uri, params string [] segments )
        {
            if (string.IsNullOrWhiteSpace(uri))
                return null;

            if (segments == null || segments.Length == 0)
                return uri;

            return segments.Aggregate(uri, ( current, segment ) => $"{current.TrimEnd('/')}/{segment.TrimStart('/')}");
        }

    }
}
