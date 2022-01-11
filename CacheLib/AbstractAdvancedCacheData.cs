using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib
{
    public abstract class AbstractAdvancedCacheData<TKey, TValue>
    {
        public TKey Key { get; internal set; }
        public TValue Value { get; internal set; }

        protected AbstractAdvancedCacheData()
        {
        }

        public abstract void OnFetch();
        public abstract void OnSet();
        public abstract void OnDelete();
        public abstract void OnCompareAndSwap();
    }
}
