using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSVToESLib
{
    public class CSVImporter
    {
        CSVClient CSVClient = new CSVClient();
        ElasticsearchClient ElasticsearchClient = new ElasticsearchClient();

        public async Task<bool> ImportCSV(string filePath)
        {
            var result = await ElasticsearchClient.BulkInsert(CSVClient.Parse(filePath));
            return result.Success;
        }
    }
}
