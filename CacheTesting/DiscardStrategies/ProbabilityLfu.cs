using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib.Discard;

namespace CacheTesting.DiscardStrategies
{
    public class ProbabilityLfu : IDiscardPolicy<BasicAccessData>
    {
        private readonly Random _rand;
        private readonly double _probability;

        public virtual double ProbabilityThreshold { get; } = 0.75;

        public virtual Dimension ThisDimension { get; } = (Dimension) 1;

        public int LookAhead { get; } = 1;

        public ProbabilityLfu()
        {
            _probability = Math.Pow(ProbabilityThreshold, 0.1);
            _rand = new Random(42);
        }

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
            return null;
        }

        public Dimension ChangeTo(object clusterData, BasicAccessData value)
        {
            if (_probability <= _rand.NextDouble())
            {
                return ThisDimension;
            }

            return Dimension.NoChange;
        }

        public bool Allowance(object clusterData, BasicAccessData value)
        {
            return true;
        }
    }

    public class ProbabilityLfuBottomLru : SimpleLru
    {
        public override Dimension ThisDimension { get; } = (Dimension) 2;
    }

    public class ProbabilityLfuBottomRand : RandomPol
    {
        public override Dimension ThisDimension { get; } = (Dimension) 2;
    }
}
