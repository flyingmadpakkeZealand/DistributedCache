using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib.Discard;

namespace CacheTesting.DiscardStrategies
{
    public class RandomPol : IDiscardPolicy<BasicAccessData>
    {
        private readonly Random _rand;

        public virtual Dimension ThisDimension { get; } = (Dimension) 3;

        public int LookAhead { get; } = 1;

        public RandomPol()
        {
            _rand = new Random(42);
        }

        public ClusterPosition Insertion(BasicAccessData value)
        {
            return _rand.Next(2) == 0 ? ClusterPosition.First : ClusterPosition.Last;
        }

        public ClusterPosition Deletion()
        {
            return _rand.Next(2) == 0 ? ClusterPosition.First : ClusterPosition.Last;
        }

        public object ClusterData(BasicAccessData value)
        {
            return null;
        }

        public Dimension ChangeTo(object clusterData, BasicAccessData value)
        {
            return _rand.Next(2) == 0 ? ThisDimension : Dimension.NoChange;
        }

        public bool Allowance(object clusterData, BasicAccessData value)
        {
            return true;
        }
    }
}
