using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSVToESLib.Template
{
    class CsvImporter : ICsvImporter
    {
        CSVClient CSVClient = new CSVClient();
        ElasticsearchClient ElasticsearchClient = new ElasticsearchClient();

        public async Task<bool> ImportCSV(string filePath, int version)
        {
            var result = await ElasticsearchClient.BulkInsert(CSVClient.Parse(filePath));
            return result.Success;
        }
    }
}
