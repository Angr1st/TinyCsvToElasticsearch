using System.Threading.Tasks;
using Elasticsearch.Net;

namespace CSVToESLib.Template
{
    class CsvImporter : ICsvImporter
    {
        CSVClient CSVClient = new CSVClient();

        public async Task<bool> ImportCsv(ConnectionConfiguration connection, string filePath, int version)
        {
            var ElasticsearClient = new ElasticsearchClient(connection);
            var result = await ElasticsearClient.BulkInsert(CSVClient.Parse(filePath));
            return result.Success;
        }
    }
}
