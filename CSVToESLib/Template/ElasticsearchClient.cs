using CSVToESLib.Types;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSVToESLib.Template
{
    internal class ElasticsearchClient
    {
        private readonly ElasticClient _elasticClient;

        public ElasticsearchClient(IConnectionSettingsValues settings) => _elasticClient = new ElasticClient(settings);

        public async Task<Result<int, Exception>> BulkInsert<T>(ElasticsearchConnection con, IEnumerable<T> results) where T : class
        {
            try
            {
                var bulkAllObservable = _elasticClient.BulkAll(results, b => b
                    .Index(IndexName.From<T>())
                    .Type(TypeName.From<T>())
                    .RefreshOnCompleted()
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(5000)
                );
                bulkAllObservable.Subscribe(con.BulkAllObserver);
                await con.WaitForCompletion();

                return new Result<int, Exception>(con.RecordCount);
            }
            catch (Exception e)
            {
                return new Result<int, Exception>(e);
            }
        }
    }
}
