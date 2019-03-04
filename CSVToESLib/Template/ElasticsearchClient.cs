using Elasticsearch.Net;
using System;
using System.Collections.Generic;
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
            var test = results.Where(result => result.IsValid).Select(result => { result.Result.Version = version; return new Index(result.Result).ToString(); });
            var postData = PostData.MultiJson(test);

            postData.Write(System.IO.File.OpenWrite("Output2.txt"), new ConnectionConfiguration(uri:null));
            //System.IO.File.WriteAllLines("output.txt", postData., System.Text.Encoding.UTF8);

            return await ElasticLowLevelClient.BulkAsync<StringResponse>(postData);
        }
    }

    public class Index
    {
        public InnerIndex InnerIndex;
        public Person Person;

        public Index(Person person)
        {
            Person = person;
            InnerIndex = new InnerIndex();
        }

        public override string ToString()
        {
            return InnerIndex.ToString() + "\n" + Person.ToString() + "\n";
        }
    }

    public class InnerIndex
    {
        public string _index;
        public string _type;

        public InnerIndex(string index = null, string type = null)
        {
            _index = index;
            _type = type;
        }

        public override string ToString()
        {
            return "{\"index\":{\"_index\":\"persons\",\"_type\":\"person\"}}";
        }
    }
}
