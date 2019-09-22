using System;
using System.Collections.Generic;
using System.Text;

namespace CSVToESLib.Constants
{
    public static class CsvImportClassNames
    {
        public static readonly string CsvClient = nameof(CsvClient);
        public static readonly string CsvBulkPriceMapping = nameof(CsvBulkPriceMapping);
        public static readonly string BulkPrice = nameof(BulkPrice);
        public static readonly string ElasticsearchClient = nameof(ElasticsearchClient);
        public static readonly string Index = nameof(Index);
        public static readonly string InnerIndex = nameof(InnerIndex);
        public static readonly string CsvMapping = nameof(CsvMapping);
        public static readonly string _index = nameof(_index);
        public static readonly string _type = nameof(_type);
        public static readonly string CsvImporter = nameof(CsvImporter);
        public static readonly string ICsvImporter = nameof(ICsvImporter);
    }
}
