using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Utilities
{
    public static class TypeHelper
    {
        public static readonly Dictionary<Type, string> KeywordDictionary = new()
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(object), "object" }
        };

        public static string GetTypeString(Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type nullableType)
            {
                return $"{GetTypeString(nullableType)}?";
            }
            if (type.IsGenericType)
            {
                Type[] genericArgs = type.GetGenericArguments();

                string name = type.Name.Split('`')[0];
                return $"{name}<{string.Join(", ", genericArgs.Select(GetTypeString))}>";
            }
            if (type == typeof(EventCallback) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventCallback<>)))
            {
                return nameof(EventCallback);
            }

            return KeywordDictionary.GetValueOrDefault(type, type.Name);
        }
    }
}
