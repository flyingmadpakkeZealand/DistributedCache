using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib;

namespace CacheTesting
{
    public abstract class TestScenarioBase
    {
        protected Random _rand;

        public int Seed { get;}
        public int Iterations { get; }
        public ICache<int, int> Cache { get; private set; }

        protected TestScenarioBase(int seed, int iterations, ICache<int, int> cache)
        {
            Seed = seed;
            Iterations = iterations;
            Cache = cache;
            _rand = new Random(seed);
        }

        public (int allCacheMisses, int trueCacheMisses, int recurringRequestCount) RunTest()
        {
            HashSet<int> archivedRequests = new HashSet<int>();
            int allCacheMisses = 0;
            int trueCacheMisses = 0;
            int recurringRequestCount = 0;

            for (int i = 0; i < Iterations; i++)
            {
                int request = NextRequest();

                if (!Cache.Fetch(request, out int _))
                {
                    allCacheMisses++;

                    if (archivedRequests.Contains(request))
                    {
                        trueCacheMisses++;
                    }

                    Cache.Set(request, request);
                }

                if (archivedRequests.Contains(request))
                {
                    recurringRequestCount++;
                }
                else
                {
                    archivedRequests.Add(request);
                }
            }

            return (allCacheMisses, trueCacheMisses, recurringRequestCount);
        }

        public void Reset(ICache<int, int> cache)
        {
            _rand = new Random(Seed);
            Cache = cache;
            Reset();
        }

        protected abstract int NextRequest();

        protected abstract void Reset();
    }
}
