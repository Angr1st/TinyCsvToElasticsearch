using Elasticsearch.Net;
using System.Threading.Tasks;
using Nest;

namespace CSVToESLib
{
    public interface ICsvImporter
    {
        void ImportCsv(IConnectionSettingsValues settings, string filePath, int version);
    }
}
