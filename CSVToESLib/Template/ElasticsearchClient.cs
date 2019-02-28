using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace CSVToESLib
{
    internal class ElasticsearchClient
    {
        ElasticLowLevelClient ElasticLowLevelClient { get; set; }
        public ElasticsearchClient(ConnectionConfiguration configuration)
        {
            ElasticLowLevelClient = new ElasticLowLevelClient(configuration);
        }


        public async Task<StringResponse> BulkInsert(ParallelQuery<CsvMappingResult<Person>> results)
        {
            return await ElasticLowLevelClient.BulkAsync<StringResponse>(PostData.MultiJson(results.Where(result => result.IsValid).Select(result => new Object[] { new Index(result.Result) }).Aggregate((newValue, oldValue) => oldValue.Concat(newValue).ToArray())));
        }
    }

    internal class Index
    {
        public InnerIndex InnerIndexField;
        public Person Person;

        public Index(Person person)
        {
            Person = person;
            InnerIndexField = new InnerIndex();
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
