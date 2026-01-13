using System;
using System.Security.Cryptography;
using System.Text;

namespace CalciAI.Crypto
{
    public static class SslEncryptor
    {
        private const string ALGORITHM = "SHA256";
        public static string SignData(string message, string key)
        {
            //// The array to store the signed message in bytes
            byte[] signedBytes;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                //// Write the message to a byte array using UTF8 as the encoding.
                var encoder = new UTF8Encoding();
                byte[] originalData = encoder.GetBytes(message);

                try
                {
                    // Import the private key used for signing the message
                    rsa.ImportFromPem(key.ToCharArray());

                    //// Sign the data, using SHA512 as the hashing algorithm 
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID(ALGORITHM));
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    // Set the keycontainer to be cleared when rsa is garbage collected.
                    rsa.PersistKeyInCsp = false;
                }
            }
            // Convert the a base64 string before returning
            return Convert.ToBase64String(signedBytes);
        }

        public static bool VerifyData(string originalMessage, string signedMessage, string publicKey)
        {
            bool success = false;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                var encoder = new UTF8Encoding();
                byte[] bytesToVerify = encoder.GetBytes(originalMessage);

                byte[] signedBytes = Convert.FromBase64String(signedMessage);
                try
                {
                    rsa.ImportFromPem(publicKey.ToCharArray());
                    success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID(ALGORITHM), signedBytes);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return success;
        }
    }
}