using System.Threading.Tasks;
using Elasticsearch.Net;

namespace CSVToESLib.Template
{
    public class CsvImporter : ICsvImporter
    {
        public async Task<StringResponse> ImportCsv(ConnectionConfiguration connection, string filePath, int version)
        {
            var CSVClient = new CsvClient();
            var ElasticsearClient = new ElasticsearchClient(connection);
            return await ElasticsearClient.BulkInsert(CSVClient.Parse(filePath),version);
        }
    }
}
