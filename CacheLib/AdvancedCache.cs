using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib.Discard;
using CacheLib.Expiry;

namespace CacheLib
{
    public class AdvancedCache<TKey, TValue, TGraph> : ICache<TKey, TValue>, IDisposable where TGraph : AbstractAdvancedCacheData<TKey, TValue>, new()
    {
        private readonly ConcurrentDictionary<TKey, LinkedListNode<object>> _dataStore;
        private readonly DiscardGraph<TGraph> _discardGraph;
        private readonly ExpiryController<TKey, TValue> _expiryController;
        private readonly IEqualityComparer<TValue> _comparer;
        private readonly int _maxSize;

        private readonly object _simpleLock = new object();

        public AdvancedCache(int maxSize, params IDiscardPolicy<TGraph>[] policies)
        {
            _discardGraph = new DiscardGraph<TGraph>(policies);
            _expiryController = ExpiryController<TKey, TValue>.CreateExpiryController(1_000, this);
            _maxSize = maxSize;

            _dataStore = new ConcurrentDictionary<TKey, LinkedListNode<object>>(8, 256);
        }

        public bool Fetch(TKey key, out TValue value)
        {
            bool keyExists = _dataStore.TryGetValue(key, out LinkedListNode<object> cacheDataNode);

            if (!keyExists)
            {
                value = default;
                return false;
            }

            lock (_simpleLock)
            {
                TGraph cacheData = (TGraph) cacheDataNode.Value;
                cacheData.OnFetch();

                value = cacheData.Value;
                return true;
            }
        }

        public bool Set(TKey key, TValue value)
        {
            _dataStore[key] = value;
            DiscardKeyIfFull();
            return true;
        }

        public bool Set(TKey key, TValue newValue, out TValue oldValue)
        {
            while (true)
            {
                bool keyExists = _dataStore.TryGetValue(key, out oldValue);

                if (_dataStore.TryUpdate(key, newValue, oldValue)
                    || !keyExists && _dataStore.TryAdd(key, newValue))
                {
                    DiscardKeyIfFull();
                    return true;
                }
            }
        }

        public bool Delete(TKey key, out TValue value) => _dataStore.TryRemove(key, out value);

        public bool Delete(TKey key) => _dataStore.TryRemove(key, out TValue _);

        public bool CompareAndSwap(TKey key, TValue expected, TValue newValue)
        {
            bool updateSucceeded = _dataStore.TryUpdate(key, newValue, expected);
            if (updateSucceeded || !Equals(expected, default(TValue))) return updateSucceeded;
            updateSucceeded = _dataStore.TryAdd(key, newValue);
            if (updateSucceeded) DiscardKeyIfFull();

            return updateSucceeded;
        }

        public void Dispose()
        {
            _expiryController.Dispose();
        }
    }
}
