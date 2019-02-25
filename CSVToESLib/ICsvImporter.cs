using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSVToESLib
{
    public interface ICsvImporter
    {
        Task<bool> ImportCSV(string filePath);
    }
}
