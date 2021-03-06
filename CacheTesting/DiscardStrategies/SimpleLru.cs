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
        public virtual Dimension ThisDimension { get; } = (Dimension) 1;

        public int LookAhead { get; } = 1;

        public ClusterPosition Insertion(BasicAccessData value)
        {
            return ClusterPosition.First;
        }

        public ClusterPosition Deletion()
        {
            return ClusterPosition.Last;
        }

        public object ClusterData(BasicAccessData value)
        {
            return null;
        }

        public virtual Dimension ChangeTo(object clusterData, BasicAccessData value)
        {
            return ThisDimension;
        }

        public bool Allowance(object clusterData, BasicAccessData value)
        {
            return true;
        }
    }
}