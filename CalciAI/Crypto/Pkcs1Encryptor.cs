using System;
using System.Security.Cryptography;
using System.Text;

namespace CalciAI.Crypto
{
    /// <summary>
    ///     var payload = new
    ///     {
    ///     	user = "jac",
    ///     	token = "RDE:Jack:T1",
    ///     	partner_id = "radhe"
    ///     };
    ///     
    ///     var strPayload = System.Text.Json.JsonSerializer.Serialize(payload);
    ///     
    ///     var config = FileConfigProvider.Load<HttpClientConfig>("dream");
    ///     
    ///     var privateKey = config.ExtraParams["privateKey"];
    ///     
    ///     var publicKey = config.ExtraParams["publicKey"];
    ///     
    ///     var signedHash = Pkcs1Encryptor.SignData(strPayload, privateKey);
    ///     
    ///     Console.WriteLine(Pkcs1Encryptor.VerifyData(strPayload, signedHash, publicKey));
    ///
    /// </summary>
    public static class Pkcs1Encryptor
    {
        private const string ALGORITHM = "SHA256";

        public static string SignData(string message, string privateKey)
        {
            try
            {
                //Create a new instance of RSA.
                using RSA rsa = RSA.Create();

                //The hash to sign.
                byte[] hash;
                using (SHA256 sha256 = SHA256.Create())
                {
                    var encoder = new UTF8Encoding();
                    byte[] originalData = encoder.GetBytes(message);

                    hash = sha256.ComputeHash(originalData);
                }

                rsa.ImportFromPem(privateKey.ToCharArray());

                //Create an RSASignatureFormatter object and pass it the 
                //RSA instance to transfer the key information.
                RSAPKCS1SignatureFormatter RSAFormatter = new(rsa);

                //Set the hash algorithm to SHA256.
                RSAFormatter.SetHashAlgorithm(ALGORITHM);

                //Create a signature for HashValue and return it.
                byte[] SignedHash = RSAFormatter.CreateSignature(hash);

                return Convert.ToBase64String(SignedHash);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static bool VerifyData(string originalMessage, string signedMessage, string publicKey)
        {
            try
            {
                //Create a new instance of RSA.
                using RSA rsa = RSA.Create();

                //The hash to sign.
                byte[] hash;
                using (SHA256 sha256 = SHA256.Create())
                {
                    var encoder = new UTF8Encoding();
                    byte[] originalData = encoder.GetBytes(originalMessage);

                    hash = sha256.ComputeHash(originalData);
                }

                rsa.ImportFromPem(publicKey.ToCharArray());

                //Create an RSAPKCS1SignatureDeformatter object and pass it the  
                //RSA instance to transfer the key information.
                RSAPKCS1SignatureDeformatter RSADeformatter = new(rsa);
                RSADeformatter.SetHashAlgorithm(ALGORITHM);

                //Verify the hash and display the results to the console. 
                if (RSADeformatter.VerifySignature(hash, Convert.FromBase64String(signedMessage)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}