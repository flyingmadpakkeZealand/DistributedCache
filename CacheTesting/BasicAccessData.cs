using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheTesting
{
    public class BasicAccessData : AbstractAdvancedCacheDataObject
    {
        public int FetchCount { get; set; }

        internal override void OnFetch()
        {
            FetchCount++;
        }

        internal override void OnSet()
        {
            FetchCount = 0;
        }
    }
}
