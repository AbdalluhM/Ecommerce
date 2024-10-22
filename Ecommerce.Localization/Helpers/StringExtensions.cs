using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ecommerce.Localization.Helpers
{
    public static class StringExtentions
    {
       
        public static string PrefixSuffixArray( this string inputString, string prefix = "", string suffix = "" )
        {
            if (string.IsNullOrWhiteSpace(inputString))
                return string.Empty;
            return inputString.Replace(inputString, prefix + inputString + suffix);
        }
        public static string [] PrefixSuffixArray( this string [] arr, string prefix = "", string suffix = "" )
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr [i] = arr [i].Replace(arr [i], prefix + arr [i] + suffix);
            }
            return arr;
        }
        public static string [] PrefSuffArray( this object [] arr, string prefix = "", string suffix = "" )
        {
            var args = arr != null && arr.Length > 0 ? new string [arr.Length] : new string [0];
            for (int i = 0; i < arr.Length; i++)
            {
                args [i] = arr [i].ToString().Replace(arr [i].ToString(), prefix + arr [i] + suffix);
            }
            return args;
        }
    }

}
