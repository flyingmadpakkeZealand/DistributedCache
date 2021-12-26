using System;
using CacheLib;

namespace CacheTesting.Scenarios
{
    public class RandomRequestsTest : TestScenarioBase
    {
        private readonly Range _requestRange;

        public RandomRequestsTest(Range range, int seed, int iterations, ICache<int, int> cache) : base(seed, iterations, cache)
        {
            _requestRange = range;
        }

        protected override int NextRequest()
        {
            return _rand.Next(_requestRange.Start.Value, _requestRange.End.Value);
        }

        protected override void Reset()
        {
            
        }
    }
}
