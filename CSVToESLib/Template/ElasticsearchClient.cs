using Elasticsearch.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace CSVToESLib.Template
{
    internal class ElasticsearchClient
    {
        ElasticLowLevelClient ElasticLowLevelClient { get; set; }
        public ElasticsearchClient(ConnectionConfiguration configuration)
        {
            ElasticLowLevelClient = new ElasticLowLevelClient(configuration);
        }

        public async Task<StringResponse> BulkInsert(ParallelQuery<CsvMappingResult<Person>> results, int version)
        {
            return await ElasticLowLevelClient.BulkAsync<StringResponse>(PostData.MultiJson(results.Where(result => result.IsValid).Select(result =>{ result.Result.Version = version; return new Object[] { new Index(result.Result) }; }).Aggregate((newValue, oldValue) => oldValue.Concat(newValue).ToArray())));
        }
    }

    internal class Index
    {
        public InnerIndex InnerIndex;
        public Person Person;

        public Index(Person person)
        {
            Person = person;
            InnerIndex = new InnerIndex();
        }
    }

    internal class InnerIndex
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
