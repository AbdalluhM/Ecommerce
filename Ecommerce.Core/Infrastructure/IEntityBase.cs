namespace Ecommerce.Core.Entities
{
    public interface IEntityBase
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets a value indicating whether the isActive. </summary>
        ///
        /// <value> True if isActive, false if not. </value>
        ///-------------------------------------------------------------------------------------------------
        bool IsActive { get; set; }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets a value indicating whether this object is deleted. </summary>
        ///
        /// <value> True if this object is deleted, false if not. </value>
        ///-------------------------------------------------------------------------------------------------
        bool IsDeleted { get; set; }
    }
}