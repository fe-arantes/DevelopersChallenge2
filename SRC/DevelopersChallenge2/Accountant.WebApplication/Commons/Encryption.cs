using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Accountant.WebApplication.Commons
{
    public static class Encryption
    {
        const string hashKey = @"r5NYMhDSnWDNigF0VyuhSF@bC7s1KX";

        private static byte[] GetHashKey()
        {
            var salt = "Nibo Challenge Salt";
            var encoder = new UTF8Encoding();
            var saltBytes = encoder.GetBytes(salt);

            var rfc = new Rfc2898DeriveBytes(hashKey, saltBytes);
            return rfc.GetBytes(16);
        }

        public static string Encrypt(string dataToEncrypt)
        {
            var hashKey = GetHashKey();
            var encryptor = new AesManaged
            {
                Key = hashKey,
                IV = hashKey
            };

            using var encryptionStream = new MemoryStream();
            using var encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write);

            var utfD1 = Encoding.UTF8.GetBytes(dataToEncrypt);
            encrypt.Write(utfD1, 0, utfD1.Length);
            encrypt.FlushFinalBlock();
            encrypt.Close();

            var bytes = encryptionStream.ToArray();
            return string.Concat(bytes.Select(b => b.ToString("X2")).ToArray());
        }

        public static string Decrypt(string encryptedX2String)
        {
            var hashKey = GetHashKey();
            var decryptor = new AesManaged
            {
                Key = hashKey,
                IV = hashKey
            };

            var encryptedData = Enumerable.Range(0, encryptedX2String.Length).
                                           Where(x => x % 2 == 0).
                                           Select(x => Convert.ToByte(encryptedX2String.Substring(x, 2), 16)).ToArray();

            using var decryptionStream = new MemoryStream();
            using var decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write);
            decrypt.Write(encryptedData, 0, encryptedData.Length);
            decrypt.Flush();
            decrypt.Close();

            var decryptedData = decryptionStream.ToArray();
            return Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
        }

    }
}
