using Elasticsearch.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace CSVToESLib
{
    class ElasticsearchClient
    {
        Elasticsearch.Net.ElasticLowLevelClient ElasticLowLevelClient = new Elasticsearch.Net.ElasticLowLevelClient();

        public async Task<StringResponse> BulkInsert(ParallelQuery<CsvMappingResult<Person>> results)
        {
            return await ElasticLowLevelClient.BulkAsync<StringResponse>(PostData.MultiJson(results.Where(result => result.IsValid).Select(result => new Object[] { new Index(result.Result) }).Aggregate((newValue, oldValue) => oldValue.Concat(newValue).ToArray())));
        }
    }

    class Index
    {
        public InnerIndex InnerIndexField;
        public Person Person;

        public Index(Person person)
        {
            Person = person;
            InnerIndexField = new InnerIndex();
        }
    }

    class InnerIndex
    {
        public string _index;
        public string _type;

        public InnerIndex(string index = "persons", string type = "person")
        {
            _index = index;
            _type = type;
        }
    }
}
