using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Common
{
    public static class StringHelper
    {
        public static string GenerateRandomNumber
        {
            get
            {
                // Generates a random number between 100000 and 999999 (inclusive).
                Random random = new Random();
                return random.Next(100000, 999999).ToString();
            }
           
        }

        public static string EncryptString(string plainText)
        {

            string key = "6ae8af956ba382f9c1e2a89ffd15aff2";
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new Byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int byteparse);
        }


        public static string DecryptString(string cipherText)
        {
            try
            {
                if (!IsBase64String(cipherText))
                {
                    return cipherText;
                }
                string key = "6ae8af956ba382f9c1e2a89ffd15aff2";
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);
                string Rval = "";
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                //return streamReader.ReadToEnd();
                                Rval = streamReader.ReadToEnd();
                            }
                        }
                    }
                }
                return Rval;
            }
            catch (Exception ex)
            {
                return cipherText;
            }
        }

        public static List<Dictionary<string, string>> ExtractMetaTags(string url)
        {
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);

                var metaTags = new List<Dictionary<string, string>>();

                // Select all meta tags
                var nodes = doc.DocumentNode.SelectNodes("//meta");

                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        var tag = new Dictionary<string, string>();
                        foreach (var attribute in node.Attributes)
                        {
                            tag[attribute.Name] = attribute.Value;
                        }
                        metaTags.Add(tag);
                    }
                }

                return metaTags;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching or parsing HTML: " + ex.Message);
                return null;
            }
        }
    }

}
