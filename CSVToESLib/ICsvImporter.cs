using Elasticsearch.Net;
using System.Threading.Tasks;
using Nest;
using System;
using CSVToESLib.Template;

namespace CSVToESLib
{
    public interface ICsvImporter
    {
        Task<Result<int, Exception>> ImportCsv(IConnectionSettingsValues settings, string filePath, int version);
    }
}
