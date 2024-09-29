using NUnit.Framework;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace dynamics365accelerator.Support.Utils
{
    internal class Cryptography
    {
        public static string EncryptString(string plainText)
        {
            var config = EnvConfig.Get();

            if (string.IsNullOrEmpty(config.GetSection("D3654_KEY").Value))
               Assert.Fail("Environment variable 'D365_KEY' is  missing or empty - this is the encryption key");

            ArgumentNullException.ThrowIfNull(plainText);

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())   
            {
                aes.Key = Encoding.UTF8.GetBytes(config.GetSection("D3654_KEY").Value);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }


        public static string DecryptString(string cipherText)
        {
            var config = EnvConfig.Get();

            if (string.IsNullOrEmpty(config.GetSection("D3654_KEY").Value))
               Assert.Fail("Environment variable 'D365_KEY' is  missing or empty - this is the encryption key");

            ArgumentNullException.ThrowIfNull(cipherText);

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())   
            {
                aes.Key = Encoding.UTF8.GetBytes(config.GetSection("D3654_KEY").Value);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                        
                    }
                }
            }
            //Completed

        }
    }
}