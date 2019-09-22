using Elasticsearch.Net;
using System.Threading.Tasks;
using Nest;
using System;
using CSVToESLib.Types;

namespace CSVToESLib.Interfaces
{
    public interface ICsvImporter
    {
        Task<Result<int, Exception>> ImportCsv(IConnectionSettingsValues settings, string filePath, int version, int chunksize = 5000);
    }
}
