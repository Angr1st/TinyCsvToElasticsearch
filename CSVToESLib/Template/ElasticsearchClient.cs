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
             
        public ElasticsearchClient(IConnectionSettingsValues settings) => 
            _elasticClient = new ElasticClient(settings);

        public void BulkInsert<T>(IEnumerable<T> results) where T : class
        {
            var bulkAllObservable = _elasticClient.BulkAll(results, b => b
                .Index(IndexName.From<T>())
                .Type(TypeName.From<T>())
                .RefreshOnCompleted()
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(5000)
            );

            Exception exception = null;
            var waitHandle = new ManualResetEvent(false);
            
            var bulkAllObserver = new BulkAllObserver(
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

            bulkAllObservable.Subscribe(bulkAllObserver);

            waitHandle.WaitOne(); // might want to pass in a timeout in method for this

            if (exception != null)
                throw exception;
        }
    }
}
