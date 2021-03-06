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

        private void DiscardKeyIfFull()
        {
            if (_dataStore.Count <= _maxSize) return;

            ICollection<TKey> keyCollection = _dataStore.Keys;
            TKey firstKey = keyCollection.First();
            _dataStore.TryRemove(firstKey, out TValue _);
        }
    }
}