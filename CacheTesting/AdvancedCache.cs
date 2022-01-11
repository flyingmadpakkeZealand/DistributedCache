using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib;
using CacheLib.Discard;

namespace CacheTesting
{
    public class AdvancedCache<TGraph> : ICache<int, int> where TGraph : AbstractAdvancedCacheDataObject, new()
    {
        private readonly SimpleCache<int, LinkedListNode<object>> _simpleCache;
        private readonly DiscardGraph<TGraph> _discardGraph;

        public int MaxSize { get; }
        public int Size { get; private set; }

        public AdvancedCache(DiscardGraph<TGraph> discardGraph, int maxSize)
        {
            Size = 0;
            MaxSize = maxSize;
            _discardGraph = discardGraph;
            _simpleCache = new SimpleCache<int, LinkedListNode<object>>(int.MaxValue);
        }

        public bool Fetch(int key, out int value)
        {
            bool success = _simpleCache.Fetch(key, out LinkedListNode<object> node);

            if (success)
            {
                TGraph nodeValue = (TGraph) node.Value;
                nodeValue.OnFetch();
                value = nodeValue.Value;
                _discardGraph.UpdateNode(ref node);

                return true;
            }

            value = default;
            return false;
        }

        public bool Set(int key, int value)
        {
            if (Size >= MaxSize)
            {
                BasicAccessData removed = (BasicAccessData) _discardGraph.RemoveEntries().First().Value;
                Size--;
                _simpleCache.Delete(removed.Key);
            }

            TGraph dataObject = new TGraph {Value = value, Key = key};

            LinkedListNode<object> node = _discardGraph.AddNew(dataObject);
            Size++;
            dataObject.OnSet();

            return _simpleCache.Set(key, node);
        }

        public bool Set(int key, int newValue, out int oldValue)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int key, out int value)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int key)
        {
            throw new NotImplementedException();
        }

        public bool CompareAndSwap(int key, int expected, int newValue)
        {
            throw new NotImplementedException();
        }
    }
}
