using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib.Discard;

namespace CacheTesting.DiscardStrategies
{
    public class ProbabilityLfRu : SimpleLfRu
    {
        
    }

    public class ProbabilityLfRuMiddleProbLfu : ProbabilityLfu
    {
        public override Dimension ThisDimension { get; } = (Dimension) 2;
    }

    public class ProbabilityLfRuBottomLru : SimpleLru
    {
        public override Dimension ThisDimension { get; } = (Dimension) 3;
    }
}
