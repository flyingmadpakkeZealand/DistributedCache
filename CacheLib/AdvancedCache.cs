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
        private readonly int _maxSize;

        private readonly object _lock = new object();

        public AdvancedCache(int maxSize, params IDiscardPolicy<TGraph>[] policies)
        {
            _discardGraph = new DiscardGraph<TGraph>(policies);
            _expiryController = ExpiryController<TKey, TValue>.CreateExpiryController(1_000, this);
            _maxSize = maxSize;

            _dataStore = new ConcurrentDictionary<TKey, LinkedListNode<object>>(8, 256);
        }

        //These methods cannot be properly made without changing sourcecode of the concurrent dictionary.
        public bool Fetch(TKey key, out TValue value)
        {
            lock (_lock)
            {
                bool keyExists = _dataStore.TryGetValue(key, out LinkedListNode<object> cacheDataNode);

                if (!keyExists)
                {
                    value = default;
                    return false;
                }

                TGraph cacheData = (TGraph)cacheDataNode.Value;
                cacheData.OnFetch();

                value = cacheData.Value;
                return true;
            }
        }

        public bool Fetch(TKey key, out TValue value, DateTimeOffset newExpireTime)
        {
            lock (_lock)
            {
                bool fetched = Fetch(key, out value);

                if (fetched)
                {
                    _dataStore.TryGetValue(key, out LinkedListNode<object> cacheDataNode);
                    TGraph cacheData = (TGraph)cacheDataNode.Value;

                    _expiryController.AddOrUpdate(cacheData.ExpireTime, newExpireTime, key);
                    cacheData.ExpireTime = newExpireTime;
                }

                return fetched;
            }
        }

        public bool Set(TKey key, TValue value) => Set(key, value, out TValue _);

        public bool Set(TKey key, TValue newValue, out TValue oldValue)
        {
            lock (_lock)
            {
                bool keyExists = _dataStore.TryGetValue(key, out LinkedListNode<object> cacheDataNode);
                oldValue = keyExists ? ((TGraph) cacheDataNode.Value).Value : default;

                TGraph newCacheData = new TGraph {Value = newValue, Key = key};
                LinkedListNode<object> newCacheDataNode = _discardGraph.AddNew(newCacheData);

                if (_dataStore.TryUpdate(key, newCacheDataNode, cacheDataNode)
                    || !keyExists && _dataStore.TryAdd(key, newCacheDataNode))
                {
                    if (keyExists)
                    {
                        _discardGraph.QuickRemove(ref cacheDataNode);
                    }

                    newCacheData.OnSet();
                    return true;
                }

                _discardGraph.QuickRemove(ref newCacheDataNode);
                return false;
            }
        }

        public bool Set(TKey key, TValue newValue, out TValue oldValue, DateTimeOffset expireTime)
        {
            lock (_lock)
            {
                bool added = Set(key, newValue, out oldValue);

                if (added)
                {
                    _dataStore.TryGetValue(key, out LinkedListNode<object> cacheDataNode);
                    TGraph cacheData = (TGraph) cacheDataNode.Value;
                    cacheData.ExpireTime = expireTime;
                    _expiryController.AddOrUpdate(null, expireTime, key);
                }

                return added;
            }
        }

        public bool Delete(TKey key, out TValue value)
        {
            lock (_lock)
            {
                bool removed = _dataStore.TryRemove(key, out LinkedListNode<object> oldCacheDataNode);

                if (removed)
                {
                    _discardGraph.QuickRemove(ref oldCacheDataNode);
                    TGraph oldCacheData = (TGraph) oldCacheDataNode.Value;
                    value = oldCacheData.Value;
                }

                value = default;
                return removed;
            }
        }

        public bool Delete(TKey key) => Delete(key, out TValue _);

        public bool CompareAndSwap(TKey key, TValue expected, TValue newValue, DateTimeOffset expireTime)
        {
            lock (_lock)
            {
                bool swapped = CompareAndSwap(key, expected, newValue);

                if (swapped)
                {
                    _dataStore.TryGetValue(key, out LinkedListNode<object> cacheDataNode);
                    TGraph cacheData = (TGraph)cacheDataNode.Value;
                    cacheData.ExpireTime = expireTime;
                    _expiryController.AddOrUpdate(null, expireTime, key);
                }

                return swapped;
            }
        }

        public bool CompareAndSwap(TKey key, TValue expected, TValue newValue)
        {
            lock (_lock)
            {
                bool keyExists = _dataStore.TryGetValue(key, out LinkedListNode<object> actual);

                if (keyExists)
                {
                    if (((TGraph) actual.Value).Value.Equals(expected))
                    {
                        _discardGraph.QuickRemove(ref actual);

                        TGraph newCacheData = new TGraph();
                        newCacheData.Value = newValue;
                        newCacheData.Key = key;
                        LinkedListNode<object> newCacheDataNode = _discardGraph.AddNew(newCacheData);
                        newCacheData.OnSet();
                        _dataStore[key] = newCacheDataNode;

                        return true;
                    }

                    return false;
                }

                if (Equals(expected, default(TValue)))
                {
                    TGraph newCacheData = new TGraph();
                    newCacheData.Value = newValue;
                    newCacheData.Key = key;

                    LinkedListNode<object> newCacheDataNode = _discardGraph.AddNew(newCacheData);
                    if (!_dataStore.TryAdd(key, newCacheDataNode))
                    {
                        _discardGraph.QuickRemove(ref newCacheDataNode);
                        return false;
                    }
                    newCacheData.OnSet();

                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            _expiryController.Dispose();
        }
    }
}
