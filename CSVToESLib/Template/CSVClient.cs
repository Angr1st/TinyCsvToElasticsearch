using System.Linq;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace CSVToESLib.Template
{
    class CsvClient
    {
        public ParallelQuery<CsvMappingResult<Person>> Parse(string filePath)
        {
            var csvParserOptions = new CsvParserOptions(true, ';');
            var csvMapper = new CSVPersonMapping();
            var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

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

    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string BirthDay { get; set; }

        public int Version;

        public override string ToString()
        {
            return $"{{\"FirstName\":\"{FirstName}\",\"LastName\":\"{LastName}\",\"BirthDay\":\"{BirthDay}\",\"Version\":\"{Version}\"}}";
        }
    }
}
