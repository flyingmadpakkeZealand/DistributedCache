using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib
{
    public class SimpleCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _dataStore;
        private readonly IEqualityComparer<TValue> _comparer;
        private readonly int _maxSize;

        public SimpleCache(int maxSize)
        {
            _maxSize = maxSize;
            _dataStore = new ConcurrentDictionary<TKey, TValue>(8, 256);
        }

        public bool Fetch(TKey key, out TValue value) => _dataStore.TryGetValue(key, out value);

        public bool Set(TKey key, TValue value)
        {
            if (_dataStore.Count >= _maxSize) DiscardFirstKeyOnFull();

            _dataStore[key] = value;
            return true;
        }

        public bool Delete(TKey key, out TValue value) => _dataStore.TryRemove(key, out value);

        public bool Delete(TKey key) => _dataStore.TryRemove(key, out TValue _);

        public bool CompareAndSwap(TKey key, TValue expected, TValue newValue)
        {
            bool updated = _dataStore.TryUpdate(key, newValue, expected);
            if (!updated && Equals(expected, default(TValue)))
            {
                updated = _dataStore.TryAdd(key, newValue);
                if (updated && _dataStore.Count > _maxSize) DiscardFirstKeyOnFull();
            }
            
            return updated;
        }

        private void DiscardFirstKeyOnFull()
        {
            ICollection<TKey> keyCollection = _dataStore.Keys;
            TKey firstKey = keyCollection.First();
            _dataStore.TryRemove(firstKey, out TValue _);
        }
    }
}