using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Utils.Helper
{
    public class Encrypt
    {
        public static string EncryptMd5(string password)
        {
            MD5 mD5 = MD5.Create();
            byte[] input = Encoding.UTF8.GetBytes(password);
            byte[] output = mD5.ComputeHash(input);
            string passwordHashed = BitConverter.ToString(output).Replace("-", string.Empty);
            return passwordHashed;
        }
    }
}
