using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib.Discard;

namespace CacheTesting.DiscardStrategies
{
    public class SimpleLru : IDiscardPolicy<BasicAccessData>
    {
        public int LookAhead { get; } = 1;

        public int Insertion(BasicAccessData listValue)
        {
            return 0;
        }

        public int Deletion()
        {
            return 1;
        }

        public object ClusterData(BasicAccessData listValue)
        {
            return null;
        }

        public virtual int Change(object clusterData, BasicAccessData listValue)
        {
            return 1;
        }

        public bool Allowance(object clusterData, BasicAccessData listValue)
        {
            return true;
        }
    }
}