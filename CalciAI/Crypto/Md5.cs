using System;
using System.Security.Cryptography;
using System.Text;

namespace CalciAI.Crypto
{
    public static class Md5
    {
        public static string GetMd5Hash(string salt, string input)
        {
            var provider = MD5.Create();
            var bytes = provider.ComputeHash(Encoding.ASCII.GetBytes(input + salt));
            var computedHash = BitConverter.ToString(bytes);
            var computedHashWithoutDash = computedHash.Replace("-", "");
            return computedHashWithoutDash;
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(string salt, string input, string hash)
        {
            return string.Compare(GetMd5Hash(salt, input), hash, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
