using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Reflection;

namespace Euroland.NetCore.ToolsFramework.Data
{
    public sealed class TypeUtils
    {
        public static bool IsPrimitive(Type type)
        {
            // Must use Type.GetTypeInfo() in .NetCore instead of Type
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                typeInfo = type.GetGenericArguments()[0].GetTypeInfo();
            }

            return typeInfo.IsPrimitive
              || typeInfo.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal));
        }

        public static bool IsIEnumerable(Type type)
        {
            // Must use Type.GetTypeInfo() in .NetCore instead of 
            return typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Generates hash of a <see cref="Type"/>
        /// </summary>
        /// <param name="type">Type to hash</param>
        /// <returns></returns>
        public static string ToHash(Type type)
        {
            string typeStr = type.AssemblyQualifiedName + type.FullName;

            byte[] byteData = CreateHashAlgorithm().ComputeHash(System.Text.Encoding.UTF8.GetBytes(typeStr));
            StringBuilder returnString = new StringBuilder();
            foreach (var b in byteData)
            {
                returnString.Append(b.ToString("x2"));
            }
            return returnString.ToString();
        }

        static System.Security.Cryptography.HashAlgorithm hash;
        private static System.Security.Cryptography.HashAlgorithm CreateHashAlgorithm()
        {
            if (hash == null)
                hash = System.Security.Cryptography.SHA1.Create();
            return hash;
        }

    }
}
