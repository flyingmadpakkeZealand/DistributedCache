using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib
{
    public class DefaultAdvancedCacheData<TKey, TValue> : AbstractAdvancedCacheData<TKey, TValue>
    {
        public int FetchCount { get; set; }

        public override void OnFetch()
        {
            FetchCount++;
        }

        public override void OnSet()
        {
            throw new NotImplementedException();
        }

        public override void OnDelete()
        {
            throw new NotImplementedException();
        }

        public override void OnCompareAndSwap()
        {
            throw new NotImplementedException();
        }
    }
}
