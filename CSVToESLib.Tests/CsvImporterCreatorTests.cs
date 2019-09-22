using System;
using Xunit;
using CSVToESLib;
using CSVToESLib.Types;

namespace CSVToESLib.Tests
{
    public class CsvImporterGeneratorTests
    {
        public static TypeNames typeNames = new TypeNames("BulkPriceImport");

        [Fact]
        public void CachingTestDifferent()
        {
            string[] nameArray = new string[] { "SystemThreadingTasks", "ElasticsearchNet", "SystemK", "SystemLinq", "TinyCsvParserMapping", "TinyCsvParserK", "CSVToESLibK", "SystemConsole", "Person", "Test" };
            string[] newArray = new string[] { "SystemThreadingTasks1", "ElasticsearchNet", "SystemK", "SystemLinq", "TinyCsvParserMapping", "TinyCsvParserK", "CSVToESLibK", "SystemConsole", "Person", "Test" };

            var test = CsvImporterGenerator.CreateICsvImporterType(nameArray, typeNames);
            var test2 = CsvImporterGenerator.CreateICsvImporterType(newArray, typeNames);

            Assert.NotEqual(test, test2);
        }

        [Fact]
        public void ChachingTestSame()
        {
            string[] nameArray = new string[] { "SystemThreadingTasks", "ElasticsearchNet", "SystemK", "SystemLinq", "TinyCsvParserMapping", "TinyCsvParserK", "CSVToESLibK", "SystemConsole", "Person", "Test" };

            var test = CsvImporterGenerator.CreateICsvImporterType(nameArray, typeNames);
            var test2 = CsvImporterGenerator.CreateICsvImporterType(nameArray, typeNames);

            Assert.Equal(test, test2);
        }
    }
}
