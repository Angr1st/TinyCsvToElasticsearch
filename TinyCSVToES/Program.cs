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
            CSVToESLib.CsvImporterGenerator cSVImporter = new CsvImporterGenerator();


            ReadLine();
        }
    }
}
