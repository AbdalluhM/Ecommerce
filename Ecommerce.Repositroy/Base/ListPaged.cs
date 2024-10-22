using Ecommerce.Core.Paging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ecommerce.Repositroy.Base
{     ///-------------------------------------------------------------------------------------------------
      /// <summary>   A list paged. </summary>
      ///
      /// <typeparam name="T">    Generic type parameter. </typeparam>
      ///-------------------------------------------------------------------------------------------------

    public class ListPaged<T> //: List<T> where T : class
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the number of results. </summary>
        ///
        /// <value> The number of results. </value>
        ///-------------------------------------------------------------------------------------------------

        public int ResultCount { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the result. </summary>
        ///
        /// <value> The result. </value>
        ///-------------------------------------------------------------------------------------------------

        public IEnumerable<T> Result { get; set; }
    }
}
