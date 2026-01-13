using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CalciAI.Crypto
{
    /// <summary>
    /// https://jonathancrozier.com/blog/how-to-generate-a-cryptographically-secure-random-string-in-dot-net-with-c-sharp
    /// </summary>
    public static class Randomize
    {
        /// <summary>
        /// Creates a cryptographically secure random key string.
        /// </summary>
        /// <param name="count">The number of bytes of random values to create the string from</param>
        /// <returns>A secure random string</returns>
        public static string CreateSecureRandomString(int count = 32) => Regex.Replace(Convert.ToBase64String(RandomNumberGenerator.GetBytes(count)), "[^A-Za-z0-9]", "");
    }
}
