using Elasticsearch.Net;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CSVToESLib
{
    class ElasticsearchClient
    {
        Elasticsearch.Net.ElasticLowLevelClient ElasticLowLevelClient = new Elasticsearch.Net.ElasticLowLevelClient();

        public async Task<StringResponse> BulkInsert(ParallelQuery<TinyCsvParser.Mapping.CsvMappingResult<Person>> results)
        {
            return await ElasticLowLevelClient.BulkAsync<StringResponse>( PostData.MultiJson( results.Where(result => result.IsValid).Select(result => new Object[] { new { index = new { _index = "persons", _type = "person", _id = "1" } }, new { result.Result.FirstName, result.Result.LastName, result.Result.BirthDay } }).Aggregate((newValue, oldValue) => oldValue.Concat(newValue).ToArray())));
        }
    }
}
