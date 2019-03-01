using System;
using System.Collections.Generic;
using System.Text;

namespace CSVToESLib
{
    internal static class StringExtension
    {
        private static readonly string BLOCK = "BLOCK:";
        internal static string AddBlock(this string content) => BLOCK + content;
    }
}
