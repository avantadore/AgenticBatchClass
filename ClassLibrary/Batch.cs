using System;
using System.Collections.Generic;

namespace ClassLibrary
{
    internal sealed class Batch<T> : IBatch<T>
    {
        private const int MaxBatchSize = 100;
        private static readonly TimeSpan MaxItemAge = TimeSpan.FromMinutes(5);

        private readonly Queue<BatchItem> _items = new();
        private readonly object _syncRoot = new();

        public void AddItem(T item)
        {
            lock (_syncRoot)
            {
                _items.Enqueue(new BatchItem(item, DateTime.UtcNow));
            }
        }

        public IEnumerable<T> GetBatch()
        {
            lock (_syncRoot)
            {
                if (_items.Count == 0)
                {
                    return Array.Empty<T>();
                }

                if (_items.Count >= MaxBatchSize)
                {
                    return DequeueBatch(MaxBatchSize);
                }

                var oldestItemAge = DateTime.UtcNow - _items.Peek().EnqueuedAtUtc;
                if (oldestItemAge > MaxItemAge)
                {
                    return DequeueBatch(_items.Count);
                }

                return Array.Empty<T>();
            }
        }

        private List<T> DequeueBatch(int count)
        {
            var batch = new List<T>(count);
            for (var i = 0; i < count; i++)
            {
                batch.Add(_items.Dequeue().Item);
            }

            return batch;
        }

        private sealed record BatchItem(T Item, DateTime EnqueuedAtUtc);
    }
}
