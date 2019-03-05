using Elasticsearch.Net;
using System.Threading.Tasks;

namespace CSVToESLib
{
    public interface ICsvImporter
    {
        Task ImportCsv(ConnectionConfiguration connection, string filePath, int version);
    }
}
