using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib.Discard;

namespace CacheTesting.DiscardStrategies
{
    public class SimpleLfRu : IDiscardPolicy<BasicAccessData>
    {
        public static int Day = 0;

        public Dimension ThisDimension { get; } = (Dimension) 1;

        public int LookAhead { get; } = -1;

        public ClusterPosition Insertion(BasicAccessData value)
        {
            return ClusterPosition.Last;
        }

        public ClusterPosition Deletion()
        {
            return ClusterPosition.First;
        }

        public object ClusterData(BasicAccessData value)
        {
            return Day;
        }

        public Dimension ChangeTo(object clusterData, BasicAccessData value)
        {
            int day = (int) clusterData;

            if (day < Day)
            {
                value.FetchCount = value.FetchCount / 2;
                return ThisDimension;
            }

            return Dimension.NoChange;
        }

        public bool Allowance(object clusterData, BasicAccessData value)
        {
            return true;
        }
    }

    public class SimpleLfRuMiddleLfu : SimpleLfu
    {
        public override Dimension ThisDimension { get; } = (Dimension) 2;

        public override int LookAhead { get; } = 2;
    }

    public class SimpleLfRuBottomLru : SimpleLru
    {
        public override Dimension ThisDimension { get; } = (Dimension) 3;
    }
}
