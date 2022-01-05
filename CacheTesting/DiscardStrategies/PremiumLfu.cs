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
        public int LookAhead { get; } = 2;

        public int Insertion(PremiumAccessData listValue)
        {
            throw new NotImplementedException();
        }

        public int Deletion()
        {
            throw new NotImplementedException();
        }

        public object ClusterData(PremiumAccessData listValue)
        {
            throw new NotImplementedException();
        }

        public int Change(object clusterData, PremiumAccessData listValue)
        {
            throw new NotImplementedException();
        }

        public bool Allowance(object clusterData, PremiumAccessData listValue)
        {
            throw new NotImplementedException();
        }
    }
}
