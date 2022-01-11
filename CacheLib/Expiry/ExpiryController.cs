using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CacheLib.Expiry
{
    public class ExpiryController<TKey, TValue> : IDisposable
    {
        private readonly int _updateFrequencyMs;
        private readonly ConcurrentDictionary<long, HashSet<TKey>> _keyDictionary;
        private readonly ICache<TKey, TValue> _cache;

        private long _previousInterval; //As long as only 1 thread updates this at a time, it does not need synchronization.
        private volatile bool _running; //Might not need to be C# volatile, but marks that it is used cross-threads. (Although reordering past starting the thread may be bad?)

        public Task Maintainer { get; private set; }

        public static ExpiryController<TKey, TValue> CreateExpiryController(int updateFrequencyMs, ICache<TKey, TValue> cache)
        {
            ExpiryController<TKey, TValue> expiryController =
                new ExpiryController<TKey, TValue>(updateFrequencyMs, cache);

            expiryController.Maintainer = expiryController.RunExpiryMaintainerThread();

            return expiryController;
        }

        private ExpiryController(int updateFrequencyMs, ICache<TKey, TValue> cache)
        {
            if (updateFrequencyMs < 1_000)
            {
                throw new ArgumentOutOfRangeException(nameof(updateFrequencyMs), updateFrequencyMs, "Should be at least 1,000.");
            }

            _updateFrequencyMs = updateFrequencyMs;
            _keyDictionary = new ConcurrentDictionary<long, HashSet<TKey>>();
            _cache = cache;

            _previousInterval = GetInterval(DateTimeOffset.UtcNow);
        }

        public bool Add(DateTimeOffset expiryTime, TKey cacheKey)
        {
            long intervalKey = GetInterval(expiryTime);
            long syncInterval = GetInterval(DateTimeOffset.UtcNow);
            if (intervalKey < syncInterval)
            {
                _cache.Delete(cacheKey);
                return false;
            }

            _keyDictionary.TryAdd(intervalKey, new HashSet<TKey>(1));

            bool? added = _keyDictionary[intervalKey]?.Add(cacheKey);

            return added ?? false;
        }

        public bool AddOrUpdate(DateTimeOffset? oldExpiryTime, DateTimeOffset newExpiryTime, TKey cacheKey)
        {
            long newIntervalKey = GetInterval(newExpiryTime);
            long syncInterval = GetInterval(DateTimeOffset.UtcNow);

            if (oldExpiryTime is not null &&
                _keyDictionary.TryGetValue(GetInterval((DateTimeOffset) oldExpiryTime),
                    out HashSet<TKey> cacheKeys))
            {
                cacheKeys.Remove(cacheKey);
            }

            if (newIntervalKey >= syncInterval) return Add(newExpiryTime, cacheKey);

            _cache.Delete(cacheKey);

            return false;
        }

        private Task RunExpiryMaintainerThread()
        {
            _running = true;
            return Maintainer ?? Task.Run(LoopMaintainerThread);
        }

        private void LoopMaintainerThread()
        {
            while (_running)
            {
                long syncInterval = GetInterval(DateTimeOffset.UtcNow);

                MaintainKeys(syncInterval);

                if (GetInterval(DateTimeOffset.UtcNow) - syncInterval == 0)
                {
                    Thread.Sleep(_updateFrequencyMs);
                }
            }
        }

        private void MaintainKeys(long syncInterval)
        {
            for (; _previousInterval < syncInterval; _previousInterval++)
            {
                if (_keyDictionary.TryRemove(_previousInterval, out HashSet<TKey> cacheKeys))
                {
                    foreach (TKey cacheKey in cacheKeys)
                    {
                        _cache.Delete(cacheKey);
                    }
                }
            }
        }

        private long GetInterval(DateTimeOffset time)
        {
            return time.ToUnixTimeMilliseconds() / _updateFrequencyMs;
        }

        public void Dispose()
        {
            _running = false;
        }
    }
}