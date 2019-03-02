using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSVToESLib
{
    public interface ICsvImporter
    {
        Task<bool> ImportCsv(ConnectionConfiguration connection, string filePath, int version);
    }
}
