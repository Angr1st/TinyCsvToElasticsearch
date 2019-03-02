using System;
using Xunit;
using CSVToESLib;

namespace CSVToESLib.Tests
{
    public class CsvImporterGeneratorTests
    {
        [Fact]
        public void CachingTestDifferent()
        {
            string[] nameArray = new string[] { "SystemThreadingTasks", "ElasticsearchNet", "SystemK", "SystemLinq", "TinyCsvParserMapping", "TinyCsvParserK", "CSVToESLibK", "SystemConsole", "Person", "Test" };
            string[] newArray = new string[] { "SystemThreadingTasks1", "ElasticsearchNet", "SystemK", "SystemLinq", "TinyCsvParserMapping", "TinyCsvParserK", "CSVToESLibK", "SystemConsole", "Person", "Test" };

            var test = CsvImporterGenerator.CreateBulkPriceImporterType(nameArray);
            var test2 = CsvImporterGenerator.CreateBulkPriceImporterType(newArray);

            Assert.NotEqual(test, test2);
        }

        [Fact]
        public void ChachingTestSame()
        {
            string[] nameArray = new string[] { "SystemThreadingTasks", "ElasticsearchNet", "SystemK", "SystemLinq", "TinyCsvParserMapping", "TinyCsvParserK", "CSVToESLibK", "SystemConsole", "Person", "Test" };

            var test = CsvImporterGenerator.CreateBulkPriceImporterType(nameArray);
            var test2 = CsvImporterGenerator.CreateBulkPriceImporterType(nameArray);

            Assert.Equal(test, test2);
        }
    }
}
