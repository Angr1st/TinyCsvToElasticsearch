using System.Threading.Tasks;
using Elasticsearch.Net;

namespace CSVToESLib.Template
{
    public class CsvImporter : ICsvImporter
    {
        public async Task ImportCsv(ConnectionConfiguration connection, string filePath, int version)
        {
            var CSVClient = new CsvClient();
            var ElasticsearClient = new ElasticsearchClient(connection);
            await ElasticsearClient.BulkInsert(CSVClient.Parse(filePath),version);
        }
    }
}
