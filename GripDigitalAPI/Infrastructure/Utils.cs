using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GripDigitalAPI.Infrastructure
{
    public class Utils
    {
        /// <summary>
        /// Method generate SHA256 hash by key
        /// </summary>
        /// <param name="key">random key</param>
        /// <returns>Hash SHA256</returns>
        public static string GenerateHash(string key)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(key));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
