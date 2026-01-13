using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CalciAI
{
    public static class EncryptionDecryption
    {
        //public static readonly string initializationVector = "3518933FBEFAABEA";
        //public static readonly string secretKey = "6A1FE1453684E3A625A59F7CE5683PLK";

        public static readonly string initializationVector = "a3fdbfaee0594657";
        public static readonly string secretKey = "8a9109afb3946ae787527ab6f1a15b30";


        public static string EncryptText(string plainText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(secretKey);
            aesAlg.IV = Encoding.UTF8.GetBytes(initializationVector);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using StreamWriter swEncrypt = new(csEncrypt);
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string DecryptText(string cipherText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(secretKey);
            aesAlg.IV = Encoding.UTF8.GetBytes(initializationVector);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText));
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
