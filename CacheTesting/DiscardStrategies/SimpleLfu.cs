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
        public virtual Dimension ThisDimension { get; } = (Dimension) 1;

        public virtual int LookAhead { get; } = 1;
        
        public ClusterPosition Insertion(BasicAccessData value)
        {
            return ClusterPosition.First;
        }

        public ClusterPosition Deletion()
        {
            return ClusterPosition.First;
        }

        public object ClusterData(BasicAccessData value)
        {
            int interval = 10 * (value.FetchCount / 10);
            return interval < 50 ? interval : 50;
        }

        public virtual Dimension ChangeTo(object clusterData, BasicAccessData value)
        {
            int clusterCount = (int) clusterData;

            if (value.FetchCount >= clusterCount + 10 && clusterCount < 50)
            {
                return ThisDimension;
            }

            return Dimension.NoChange;
        }

        public bool Allowance(object clusterData, BasicAccessData value)
        {
            int clusterCount = (int) clusterData;

            return value.FetchCount >= clusterCount;
        }
    }

    public class SimpleLfuBottomLru : SimpleLru
    {
        public override Dimension ThisDimension { get; } = (Dimension) 2;
    }
}
