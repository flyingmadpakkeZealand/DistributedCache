using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib.Discard;

namespace CacheTesting.DiscardStrategies
{
    public class PremiumLfu : IDiscardPolicy<PremiumAccessData>
    {
        public Dimension ThisDimension { get; } = (Dimension) 1;

        public int LookAhead { get; } = 2;

        public ClusterPosition Insertion(PremiumAccessData value)
        {
            throw new NotImplementedException();
        }

        public ClusterPosition Deletion()
        {
            throw new NotImplementedException();
        }

        public object ClusterData(PremiumAccessData value)
        {
            throw new NotImplementedException();
        }

        public Dimension ChangeTo(object clusterData, PremiumAccessData value)
        {
            throw new NotImplementedException();
        }

        public bool Allowance(object clusterData, PremiumAccessData value)
        {
            throw new NotImplementedException();
        }
    }
}
