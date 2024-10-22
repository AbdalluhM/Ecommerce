///-------------------------------------------------------------------------------------------------
// file:	Security\IPasswordHasher.cs
//
// summary:	Declares the IPasswordHasher interface
///-------------------------------------------------------------------------------------------------

namespace Ecommerce.Helper.Security
{
    /// <summary>   Interface for password hasher. </summary>
    public interface IPasswordHasher
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Hash password. </summary>
        ///
        /// <param name="password"> The password. </param>
        ///
        /// <returns>   A string. </returns>
        ///-------------------------------------------------------------------------------------------------

        string HashPassword(string password);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Verify hashed password. </summary>
        ///
        /// <param name="hashedPassword">   The hashed password. </param>
        /// <param name="providedPassword"> The provided password. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        bool VerifyHashedPassword(string providedPassword, string hashedPassword);
    }
}