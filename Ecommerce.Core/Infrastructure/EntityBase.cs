using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Entities
{
    public abstract class EntityBase : IEntityBase
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets a value indicating whether the isActive. </summary>
        ///
        /// <value> True if isActive, false if not. </value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsActive { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets a value indicating whether this object is deleted. </summary>
        ///
        /// <value> True if this object is deleted, false if not. </value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsDeleted { get; set; }
    }
}
