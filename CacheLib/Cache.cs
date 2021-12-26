using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib
{
    public class Cache<TKey, TValue> : ICache<TKey, TValue>
    {
        public bool Fetch(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public bool Set(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool Delete(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public bool Delete(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool CompareAndSwap(TKey key, TValue expected, TValue newValue)
        {
            throw new NotImplementedException();
        }
    }
}
