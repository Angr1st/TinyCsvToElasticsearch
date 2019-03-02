using System;
using CSVToESLib;
using static System.Console;

namespace TinyCSVToES
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] nameArray = new string[] { "SystemThreadingTasks", "ElasticsearchNet", "SystemK", "SystemLinq", "TinyCsvParserMapping", "TinyCsvParserK", "CSVToESLibK", "SystemConsole", "Person", "Test" };
            string[] newArray = new string[] { "SystemThreadingTasks1", "ElasticsearchNet", "SystemK", "SystemLinq", "TinyCsvParserMapping", "TinyCsvParserK", "CSVToESLibK", "SystemConsole", "Person", "Test" };

            var test = CsvImporterGenerator.CreateBulkPriceImporterType(nameArray);
            var test2 = CsvImporterGenerator.CreateBulkPriceImporterType(newArray);

            ReadLine();
        }
    }
}
