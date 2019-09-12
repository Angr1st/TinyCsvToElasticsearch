using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using TinyCsvParser.Mapping;

namespace CSVToESLib.Template
{
    internal class ElasticsearchClient
    {
        private readonly ElasticClient _elasticClient;

        private readonly BulkAllObserver BulkAllObserver = new BulkAllObserver(
                onNext: b =>
                {
                    // do something e.g. write number of pages to console
                },
                onError: e =>
                {
                    exception = e;
                    waitHandle.Set();
                },
                onCompleted: () => waitHandle.Set());

        public ElasticsearchClient(IConnectionSettingsValues settings) => 
            _elasticClient = new ElasticClient(settings);

        public async Task<Result<int,Exception>> BulkInsert<T>(IEnumerable<T> results) where T : class
        {
            var bulkAllObservable = _elasticClient.BulkAll(results, b => b
                .Index(IndexName.From<T>())
                .Type(TypeName.From<T>())
                .RefreshOnCompleted()
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(5000)
            );
            bulkAllObservable.Subscribe(BulkAllObserver);
            return 
        }
    }
}
