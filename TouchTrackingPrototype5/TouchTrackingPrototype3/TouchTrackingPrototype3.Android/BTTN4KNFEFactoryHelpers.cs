using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace BTTN4KNFE
{
    public class BTTN4KNFEFactoryHelpers
    {
        // https://stackoverflow.com/questions/56744611/get-current-activity-xamarin-forms
        // https://github.com/jamesmontemagno/GifImageView-Xamarin.Android
        // https://github.com/conceptdev/xamarin-forms-samples/blob/main/EmployeeDirectoryXaml/EmployeeDirectoryXaml.Android/MainActivity.cs#L28
        public static string LoadJsonTemplate(TouchTrackingPrototype3.Droid.MainActivity activity, int resourceId)
        {
            var inputStream = activity.Resources.OpenRawResource(resourceId);
            var json = string.Empty;

            using (StreamReader sr = new StreamReader(inputStream))
            {
                json = sr.ReadToEnd();
            }

            return json;
        }

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
