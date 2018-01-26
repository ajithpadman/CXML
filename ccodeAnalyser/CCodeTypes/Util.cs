using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CCodeTypes.Util
{
    public class Util
    {


        public static string ComputeMd5ForString(string Str)
        {
            using (var md5 = MD5.Create())
            {
                byte[] CursorData = Encoding.ASCII.GetBytes(Str);
                var hash = md5.ComputeHash(CursorData);
                return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
                
            }
        }
        public static string ComputeMd5(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(file))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
                }
            }
        }
       

       
    
     
    }
}
