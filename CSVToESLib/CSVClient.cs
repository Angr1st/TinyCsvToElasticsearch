using System;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace CSVToESLib
{
    class CSVClient
    {
        public ParallelQuery<CsvMappingResult<Person>> Parse(string filePath)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
            CSVPersonMapping csvMapper = new CSVPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            return csvParser.ReadFromFile(filePath, System.Text.Encoding.UTF8);
        }
    }

    class CSVPersonMapping : CsvMapping<Person>
    {
        public CSVPersonMapping() : base()
        {
            MapProperty(0, x => x.FirstName);
            MapProperty(1, x => x.LastName);
            MapProperty(2, x => x.BirthDay);
        }
    }

    class Person
    {
        public string FirstName;

        public string LastName;

        public string BirthDay;
    }
}
