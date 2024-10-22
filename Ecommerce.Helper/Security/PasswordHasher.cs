///-------------------------------------------------------------------------------------------------
// file:	Security\PasswordHasher.cs
//
// summary:	Implements the password hasher class
///-------------------------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;

namespace Ecommerce.Helper.Security
{
    /// <summary>   
    /// Exception for signalling invalid hash errors.
    /// </summary>
    internal class InvalidHashException : Exception
    {
        /// <summary>   Default constructor. </summary>
        public InvalidHashException()
        {
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="message">  The message. </param>
        ///-------------------------------------------------------------------------------------------------

        public InvalidHashException(string message) : base(message) { }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="message">  The message. </param>
        /// <param name="inner">    The inner. </param>
        ///-------------------------------------------------------------------------------------------------

        public InvalidHashException(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>   
    /// Exception for signalling cannot perform operation errors. 
    /// </summary>
    internal class CannotPerformOperationException : Exception
    {
        /// <summary>   
        /// Default constructor. 
        /// </summary>
        public CannotPerformOperationException()
        {
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="message">  The message. </param>
        ///-------------------------------------------------------------------------------------------------

        public CannotPerformOperationException(string message)
            : base(message) { }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="message">  The message. </param>
        /// <param name="inner">    The inner. </param>
        ///-------------------------------------------------------------------------------------------------

        public CannotPerformOperationException(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>   
    /// A password hasher. 
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>   These constants may be changed without breaking existing hashes. </summary>
        public const int SALT_BYTES = 24;

        /// <summary>   The hash in bytes. </summary>
        public const int HASH_BYTES = 18;
        /// <summary>   The pbkdf 2 iterations. </summary>
        public const int PBKDF2_ITERATIONS = 64000;

        /// <summary>   These constants define the encoding and may not be changed. </summary>
        public const int HASH_SECTIONS = 5;

        /// <summary>   Zero-based index of the hash algorithm. </summary>
        public const int HASH_ALGORITHM_INDEX = 0;
        /// <summary>   Zero-based index of the iteration. </summary>
        public const int ITERATION_INDEX = 1;
        /// <summary>   Zero-based index of the hash size. </summary>
        public const int HASH_SIZE_INDEX = 2;
        /// <summary>   Zero-based index of the salt. </summary>
        public const int SALT_INDEX = 3;
        /// <summary>   Zero-based index of the pbkdf 2. </summary>
        public const int PBKDF2_INDEX = 4;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Hash password. </summary>
        ///
        /// <exception cref="CannotPerformOperationException">  Thrown when a Cannot Perform Operation
        ///                                                     error condition occurs. </exception>
        ///
        /// <param name="password"> The password. </param>
        ///
        /// <returns>   A string. </returns>
        ///-------------------------------------------------------------------------------------------------

        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SALT_BYTES];
            try
            {
                using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
                {
                    csprng.GetBytes(salt);
                }
            }
            catch (CryptographicException ex)
            {
                throw new CannotPerformOperationException(
                    "Random number generator not available.",
                    ex
                );
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to random number generator.",
                    ex
                );
            }

            byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTES);

            // format: algorithm:iterations:hashSize:salt:hash
            string parts = "sha1:" +
                PBKDF2_ITERATIONS +
                ":" +
                hash.Length +
                ":" +
                Convert.ToBase64String(salt) +
                ":" +
                Convert.ToBase64String(hash);
            return parts;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Verify hashed password. </summary>
        ///
        /// <exception cref="InvalidHashException">             Thrown when an Invalid Hash error
        ///                                                     condition occurs. </exception>
        /// <exception cref="CannotPerformOperationException">  Thrown when a Cannot Perform Operation
        ///                                                     error condition occurs. </exception>
        ///
        /// <param name="providedPassword"> The provided password. </param>
        /// <param name="hashedPassword">   The hashed password. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool VerifyHashedPassword(string providedPassword, string hashedPassword)
        {
            char[] delimiter = { ':' };
            string[] split = hashedPassword.Split(delimiter);

            if (split.Length != HASH_SECTIONS)
            {
                throw new InvalidHashException(
                    "Fields are missing from the password hash."
                );
            }

            // We only support SHA1 with C#.
            if (split[HASH_ALGORITHM_INDEX] != "sha1")
            {
                throw new CannotPerformOperationException(
                    "Unsupported hash type."
                );
            }

            int iterations = 0;
            try
            {
                iterations = Int32.Parse(split[ITERATION_INDEX]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Int32.Parse",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Could not parse the iteration count as an integer.",
                    ex
                );
            }
            catch (OverflowException ex)
            {
                throw new InvalidHashException(
                    "The iteration count is too large to be represented.",
                    ex
                );
            }

            if (iterations < 1)
            {
                throw new InvalidHashException(
                    "Invalid number of iterations. Must be >= 1."
                );
            }

            byte[] salt = null;
            try
            {
                salt = Convert.FromBase64String(split[SALT_INDEX]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Convert.FromBase64String",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Base64 decoding of salt failed.",
                    ex
                );
            }

            byte[] hash = null;
            try
            {
                hash = Convert.FromBase64String(split[PBKDF2_INDEX]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Convert.FromBase64String",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Base64 decoding of pbkdf2 output failed.",
                    ex
                );
            }

            int storedHashSize = 0;
            try
            {
                storedHashSize = Int32.Parse(split[HASH_SIZE_INDEX]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Int32.Parse",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Could not parse the hash size as an integer.",
                    ex
                );
            }
            catch (OverflowException ex)
            {
                throw new InvalidHashException(
                    "The hash size is too large to be represented.",
                    ex
                );
            }

            if (storedHashSize != hash.Length)
            {
                throw new InvalidHashException(
                    "Hash length doesn't match stored hash length."
                );
            }

            byte[] testHash = PBKDF2(providedPassword, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Slow equals. </summary>
        ///
        /// <param name="a">    The byte[] to process. </param>
        /// <param name="b">    The byte[] to process. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Pbkdf 2. </summary>
        ///
        /// <param name="password">     The password. </param>
        /// <param name="salt">         The salt. </param>
        /// <param name="iterations">   The iterations. </param>
        /// <param name="outputBytes">  The output in bytes. </param>
        ///
        /// <returns>   A byte[]. </returns>
        ///-------------------------------------------------------------------------------------------------

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
}