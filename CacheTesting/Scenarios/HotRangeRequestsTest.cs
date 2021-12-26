using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib;

namespace CacheTesting.Scenarios
{
    public class HotRangeRequestsTest : TestScenarioBase
    {
        private readonly Range _requestRange;
        private readonly Range _hotRange;
        private readonly int _hotPercentage;

        public HotRangeRequestsTest(Range requestRange, Range hotRange, int hotPercentage, int seed, int iterations, ICache<int, int> cache) : base(seed, iterations, cache)
        {
            _requestRange = requestRange;
            _hotRange = hotRange;
            _hotPercentage = hotPercentage;
        }

        protected override int NextRequest()
        {
            if (_rand.Next(0, 100) < _hotPercentage)
            {
                return _rand.Next(_hotRange.Start.Value, _hotRange.End.Value);
            }

            return _rand.Next(_requestRange.Start.Value, _requestRange.End.Value);
        }

        protected override void Reset()
        {

        }
    }
}
