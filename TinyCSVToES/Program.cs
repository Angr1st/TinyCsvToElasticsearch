using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSVToESLib;
using static System.Console;

namespace TinyCSVToES
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var filePath = "testData.csv";
            var test = CsvImporterGenerator.CreateBulkPriceImporterType(GetHeaders(filePath));

            var taskResult = await test.ImportCsv(null, filePath, 1);
            ReadLine();
        }

        public static string[] GetHeaders(string filePath) => File.ReadAllLines(filePath).First().Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
    }
}
