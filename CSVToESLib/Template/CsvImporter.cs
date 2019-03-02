using System.Threading.Tasks;
using Elasticsearch.Net;

namespace CSVToESLib.Template
{
    class CsvImporter : ICsvImporter
    {
        public async Task<bool> ImportCsv(ConnectionConfiguration connection, string filePath, int version)
        {
            var CSVClient = new CsvClient();
            var ElasticsearClient = new ElasticsearchClient(connection);
            var result = await ElasticsearClient.BulkInsert(CSVClient.Parse(filePath),version);
            return result.Success;
        }
    }
}
