using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace BTTN4KNFE
{
    public class BTTN4KNFEFactoryHelpers
    {
        static private SHA256Managed HashProvider = new SHA256Managed();

        public static byte[] ComputeHash(string s)
        {
            byte[] hash = ComputeHash(Encoding.UTF8.GetBytes(s));

            return hash;
        }

        // https://docs.microsoft.com/en-us/dotnet/standard/security/ensuring-data-integrity-with-hash-codes
        public static byte[] ComputeHash(byte[] bytes)
        {
            byte[] hash = HashProvider.ComputeHash(bytes);
            Console.WriteLine("hash:\t" + hash.Length + " " + BitConverter.ToString(hash));

            return hash;
        }
        public static string ComputeHash64(string s)
        {
            string hash64 = ComputeHash64(Encoding.UTF8.GetBytes(s));

            return hash64;
        }

        public static string ComputeHash64(byte[] bytes)
        {
            byte[] hash = ComputeHash(bytes);
            string hash64 = Convert.ToBase64String(hash);
            Console.WriteLine("hash64:\t" + hash64.Length + " " + hash64);

            return hash64;
        }
    }
}
