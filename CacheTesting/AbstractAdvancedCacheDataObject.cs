using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheTesting
{
    public abstract class AbstractAdvancedCacheDataObject
    {
        public int Key { get; internal set; }
        public int Value { get; internal set; }

        protected AbstractAdvancedCacheDataObject()
        {
        }

        internal abstract void OnFetch();
        internal abstract void OnSet();
    }
}
