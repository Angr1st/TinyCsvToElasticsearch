using System.Linq;
using Nest;

namespace CSVToESLib.Template
{
    public class CsvImporter : ICsvImporter
    {
        public void ImportCsv(IConnectionSettingsValues settings, string filePath, int version)
        {
            var csvClient = new CsvClient();
            var elasticsearchClient = new ElasticsearchClient(settings);
            var results = csvClient.Parse(filePath).Where(r => r.IsValid).Select(r =>
            {
                r.Result.Version = version;
                return r.Result;
            });

            elasticsearchClient.BulkInsert(results);
        }
    }
}