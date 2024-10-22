﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.BLL.DBQueriesDto
{
    public class CheckDto
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the has children. </summary>
        ///
        /// <value> If 0 entity doesn't have Children, else entity has Children. </value>
        ///-------------------------------------------------------------------------------------------------

        public int HasChildren { get; set; }
    }
}
