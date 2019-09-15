using System;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace CSVToESLib.Template
{
    public class CsvImporter : ICsvImporter
    {
        public async Task<Result<int, Exception>> ImportCsv(IConnectionSettingsValues settings, string filePath, int version)
        {
            var csvClient = new CsvClient();
            var elasticsearchClient = new ElasticsearchClient(settings);
            var results = csvClient.Parse(filePath).Where(r => r.IsValid).Select(r =>
            {
                r.Result.Version = version;
                return r.Result;
            });
            var elasticsearchCon = new ElasticsearchConnection();
            return await elasticsearchClient.BulkInsert(elasticsearchCon, results);
        }
    }
}