using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;

namespace Ecommerce.Helper.Extensions
{
    public static class EnumExtensions
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   An Enum extension method that converts a val to a description string. </summary>
        ///
        /// <param name="val">  The val to act on. </param>
        ///
        /// <returns>   Val as a string. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static string ToDescriptionString(this Enum val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the values in this collection. </summary>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the values in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
