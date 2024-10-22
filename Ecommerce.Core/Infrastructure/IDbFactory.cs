using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Infrastructure
{
    /// <summary>   Interface for database factory. </summary>
    public interface IDbFactory
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Initializes this object. </summary>
        ///
        /// <returns>   An ApplicationDbContext. </returns>
        ///-------------------------------------------------------------------------------------------------

        ApplicationDbContext Initialize();

    }
}
