using System;
using System.Linq;
using Nest;

namespace CSVToESLib.Template
{
    public class CsvImporter : ICsvImporter
    {
        public IDisposable ImportCsv(IConnectionSettingsValues settings, string filePath, int version, BulkAllObserver bulkAllObserver)
        {
            var csvClient = new CsvClient();
            var elasticsearchClient = new ElasticsearchClient(settings);
            var results = csvClient.Parse(filePath).Where(r => r.IsValid).Select(r =>
            {
                r.Result.Version = version;
                return r.Result;
            });

           return elasticsearchClient.BulkInsert(results, bulkAllObserver);
        }
    }
}