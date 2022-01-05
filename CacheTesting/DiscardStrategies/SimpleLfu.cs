using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib.Discard;

namespace CacheTesting.DiscardStrategies
{
    public class SimpleLfu : IDiscardPolicy<BasicAccessData>
    {
        public int LookAhead { get; } = 1;
        
        public int Insertion(BasicAccessData listValue)
        {
            return 0;
        }

        public int Deletion()
        {
            return 0;
        }

        public object ClusterData(BasicAccessData listValue)
        {
            return 10 * (listValue.FetchCount / 10);
        }

        public int Change(object clusterData, BasicAccessData listValue)
        {
            int clusterCount = (int) clusterData;

            if (listValue.FetchCount >= clusterCount + 10 && clusterCount < 50)
            {
                return 1;
            }

            return 0;
        }

        public bool Allowance(object clusterData, BasicAccessData listValue)
        {
            int clusterCount = (int) clusterData;

            return listValue.FetchCount >= clusterCount;
        }
    }

    public class SimpleLfuBottomLru : SimpleLru
    {
        public override int Change(object clusterData, BasicAccessData listValue)
        {
            return 2;
        }
    }
}
