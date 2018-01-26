using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CCodeFramework.Util
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
        public static string ComputeMd5(ClangSharp.Type type)
        {
            using (var md5 = MD5.Create())
            {
                string Type = "";
                string typekindSpelling = "";
                string isConst = "";
                string isVolatile = "";
          
                if (type != null)
                {
                    Type = type.Spelling;
                  
                    isConst = type.IsConstQualifiedType.ToString();
                    isVolatile = type.IsVolatileQualifiedType.ToString();
                    typekindSpelling = (type.TypeKindSpelling);
                   
                    
                   
                    byte[] CursorData = Encoding.ASCII.GetBytes( Type + typekindSpelling + isConst + isVolatile);
                    var hash = md5.ComputeHash(CursorData);
                    return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
                }
                else
                {
                    return "";
                }
            }
          
        }

       
        public static string ComputeElementMd5(ClangSharp.Cursor Cursor)
        {
            using (var md5 = MD5.Create())
            {
                if (Cursor != null)
                {
                    string Name = Cursor.Spelling;
                    string typeHash = "";
                    string file = "";
                    string Argument = "";
                    string ResultType = "";
                    file = Cursor.Location.File.ToString();
                    typeHash = ComputeMd5(Cursor.Type);
                    for (uint i = 0; i < Cursor.NumArguments; i++)
                    {
                        Argument += ComputeMd5(Cursor.GetArgument(i).Type);
                    }
                    ResultType = ComputeMd5(Cursor.ResultType);
                    byte[] CursorData = Encoding.ASCII.GetBytes(Name + typeHash + Argument + ResultType);
                    var hash = md5.ComputeHash(CursorData);
                    return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();

                }
                else
                {
                    return "";
                }
            }
        }
        public static string ComputeMd5(ClangSharp.Cursor Cursor)
        {
            using (var md5 = MD5.Create())
            {
                if (Cursor != null)
                {
                    string Name = Cursor.Spelling;
                    if(string.IsNullOrEmpty(Name) == true)
                    {
                        Name = ""; 
                    }
                    string typeHash = "";
                    string file = "";
                    string Argument = "";
                    string ResultType = "";
                    file = Cursor.Location.File.ToString().Replace('/','\\');
                    if(System.IO.File.Exists(file))
                    {
                        file = System.IO.Path.GetFileName(file);
                    }
                    typeHash = ComputeMd5(Cursor.Type);
                    for(uint i = 0; i < Cursor.NumArguments;i++)
                    {
                        Argument += ComputeMd5(Cursor.GetArgument(i));
                    }
                    ResultType = ComputeMd5(Cursor.ResultType);
                    byte[] CursorData = Encoding.ASCII.GetBytes(Name+ typeHash +   file+   Argument+ ResultType);
                    var hash = md5.ComputeHash(CursorData);
                    return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();

                }
                else
                {
                    return "";
                }
            }
        }
    }
}
