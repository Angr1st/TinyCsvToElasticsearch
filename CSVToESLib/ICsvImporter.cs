using Elasticsearch.Net;
using System.Threading.Tasks;
using Nest;
using System;

namespace CSVToESLib
{
    public interface ICsvImporter
    {
        IDisposable ImportCsv(IConnectionSettingsValues settings, string filePath, int version, BulkAllObserver bulkAllObserver);
    }
}
