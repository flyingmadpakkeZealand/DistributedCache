using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib;
using CacheLib.Discard;
using CacheTesting.DiscardStrategies;

namespace CacheTesting.Scenarios
{
    public class PhaseRequestsTest : TestScenarioBase
    {
        private readonly Range[] _initialPhases;
        private readonly int _phaseIncrement;
        private readonly List<Range> _allPhases;

        private int _iterationCounter;
        private int _phaseCounter;

        public PhaseRequestsTest(int seed, int iterations, ICache<int, int> cache, int phaseIncrement, params Range[] initialPhases) : base(seed, iterations, cache)
        {
            _initialPhases = initialPhases;
            _phaseIncrement = phaseIncrement;
            _allPhases = new List<Range>();
            _iterationCounter = 0;
            _phaseCounter = 0;

            foreach (Range phase in _initialPhases)
            {
                _allPhases.Add(phase);
            }
        }

        protected override int NextRequest()
        {
            _iterationCounter = ++_iterationCounter % (Iterations / 100);

            if (_iterationCounter == 0 && ++_phaseCounter % _initialPhases.Length == 0)
            {
                SimpleLfRu.Day++;

                for (int i = _initialPhases.Length; i > 0; i--)
                {
                    Range previousPhase = _allPhases[^i];
                    _allPhases.Add(new Range(previousPhase.Start.Value + _phaseIncrement, 
                        previousPhase.End.Value + _phaseIncrement));
                }
            }

            if (_rand.Next(0, 100) < 75)
            {
                Range hotPhase = _allPhases[_phaseCounter];

                return _rand.Next(hotPhase.Start.Value, hotPhase.End.Value);
            }

            Range oldPhase = _allPhases[_rand.Next(0, _allPhases.Count)];

            return _rand.Next(oldPhase.Start.Value, oldPhase.End.Value);
        }

        protected override void Reset()
        {
            _allPhases.Clear();
            _iterationCounter = 0;
            _phaseCounter = 0;
            SimpleLfRu.Day = 0;

            foreach (Range phase in _initialPhases)
            {
                _allPhases.Add(phase);
            }
        }
    }
}
