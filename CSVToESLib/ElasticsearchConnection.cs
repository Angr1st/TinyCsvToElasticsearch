using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSVToESLib
{
    public class ElasticsearchConnection
    {
        public int RecordCount { get; private set; }

        public BulkAllObserver BulkAllObserver
        {
            get => _bulkAllObserver;
            private set
            {
                _bulkAllObserver = value;
            }
        }

        private readonly TaskCompletionSource<bool> TaskCompletionSource = new TaskCompletionSource<bool>();

        private BulkAllObserver _bulkAllObserver = null;

        public ElasticsearchConnection()
        {
            BulkAllObserver = new BulkAllObserver(IncrementRecordCount, SetException, SetToComplete);
        }

        public async Task<bool> WaitForCompletion() => await TaskCompletionSource.Task;

        private void IncrementRecordCount(IBulkAllResponse response) => RecordCount++;

        private void SetException(Exception e) => TaskCompletionSource.SetException(e);

        private void SetToComplete() => TaskCompletionSource.SetResult(true);
    }
}
