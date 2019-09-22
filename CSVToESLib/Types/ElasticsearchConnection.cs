using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSVToESLib.Types
{
    public class ElasticsearchConnection
    {
        public int RecordCount { get; private set; }

        public BulkAllObserver BulkAllObserver { get; private set; } = null;

        private readonly TaskCompletionSource<bool> TaskCompletionSource = new TaskCompletionSource<bool>();

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
