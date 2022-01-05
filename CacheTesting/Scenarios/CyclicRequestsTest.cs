using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib;

namespace CacheTesting.Scenarios
{
    public class CyclicRequestsTest : TestScenarioBase
    {
        private readonly Range _firstCycle;
        private readonly Range _secondCycle;
        private readonly Range _requestRange;
        private readonly int _cyclePercentage;

        private readonly int _cycleLength;
        private int _cycleCount;
        private bool _isCooldownCycle;
        private bool _isStartCycle;

        public CyclicRequestsTest(Range requestRange, Range firstCycle, Range secondCycle, int cyclePercentage, int seed, int iterations, ICache<int, int> cache) : base(seed, iterations, cache)
        {
            _requestRange = requestRange;
            _firstCycle = firstCycle;
            _secondCycle = secondCycle;
            _cyclePercentage = cyclePercentage;
            _cycleLength = iterations / 40;
            _cycleCount = 0;

            _isCooldownCycle = false;
            _isStartCycle = true;
        }

        protected override int NextRequest()
        {
            _cycleCount = ++_cycleCount % _cycleLength;

            if (_cycleCount == 0)
            {
                if (_isCooldownCycle)
                {
                    _isStartCycle = !_isStartCycle;
                }

                _isCooldownCycle = !_isCooldownCycle;
            }

            if (_isCooldownCycle || _rand.Next(0, 100) >= _cyclePercentage)
            {
                return _rand.Next(_requestRange.Start.Value, _requestRange.End.Value);
            }

            if (_isStartCycle)
            {
                return _rand.Next(_firstCycle.Start.Value, _firstCycle.End.Value);
            }

            return _rand.Next(_secondCycle.Start.Value, _secondCycle.End.Value);
        }

        protected override void Reset()
        {
            _cycleCount = 0;

            _isCooldownCycle = false;
            _isStartCycle = true;
        }
    }
}
