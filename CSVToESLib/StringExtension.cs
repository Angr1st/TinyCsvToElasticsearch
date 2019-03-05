using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSVToESLib
{
    internal static class StringExtension
    {
        private static readonly string BLOCK = "BLOCK:";
        internal static string AddBlock(this string content) => BLOCK + content;

        internal static bool IsAssignableFrom(this Type t, Type other) => t.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());

        internal static bool AssignableFrom(this Type type, Type from) => type.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());

        internal static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        internal static byte[] Utf8Bytes(this string s) => s.IsNullOrEmpty() ? null : Encoding.UTF8.GetBytes(s);

        internal static bool HasAny<T>(this IEnumerable<T> list) => list != null && list.Any() ;

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int maxItems)
        {
            return items.Select((item, index) => new { item, index })
                        .GroupBy(x => x.index / maxItems)
                        .Select(g => g.Select(x => x.item));
        }

        public static IEnumerable<T> Batch2<T>(this IEnumerable<T> items, int skip, int take)
        {
            return items.Skip(skip).Take(take);
        }

    }
}
