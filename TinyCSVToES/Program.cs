using System;
using CSVToESLib;
using static System.Console;

namespace TinyCSVToES
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] nameArray = new string[] { "FirstFüeld", "SecondField" };
            var version = 1;
            var newImplementation = CsvImporterGenerator.CreateBulkPriceImporterType(nameArray, version);


            ReadLine();
        }
    }
}
