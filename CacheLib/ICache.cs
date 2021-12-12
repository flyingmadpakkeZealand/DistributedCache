using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib
{
    public interface ICache<in TKey, TValue>
    {
        bool Fetch(TKey key, out TValue value);

        bool Set(TKey key, TValue value);

        bool Delete(TKey key, out TValue value);

        bool Delete(TKey key);

        bool CompareAndSwap(TKey key, TValue expected, TValue newValue);
    }
}
