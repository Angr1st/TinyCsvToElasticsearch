using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSVToESLib.Extensions
{
    internal static class StringExtension
    {
        private static readonly string BLOCK = "BLOCK:";
        internal static string AddBlock(this string content) => BLOCK + content;

        internal static bool IsAssignableFrom(this Type t, Type other) => t.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());

        internal static bool AssignableFrom(this Type type, Type from) => type.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());

        internal static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        internal static byte[] Utf8Bytes(this string s) => s.IsNullOrEmpty() ? null : Encoding.UTF8.GetBytes(s);

        internal static IEnumerable<T> HasAny<T>(this IEnumerable<T> list) => list != null && list.Any() ? list : new List<T>();
    }
}
