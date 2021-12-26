using System;
using CacheLib;
using CacheLib.Discard;
using CacheTesting.DiscardStrategies;
using CacheTesting.Scenarios;

namespace CacheTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            ScenarioRunner scenarioRunner = new ScenarioRunner();

            RandomRequestsTest randomRequestsTest =
                new RandomRequestsTest(new Range(0, 150),
                    42,
                    10_000,
                    CreateSimpleLru(100));
            
            scenarioRunner.Run(randomRequestsTest, nameof(randomRequestsTest)+ "(LRU)");

            randomRequestsTest.Reset(new SimpleCache<int, int>(100));
            scenarioRunner.Run(randomRequestsTest, nameof(randomRequestsTest) + "(Control)");

            HotRangeRequestsTest hotRangeRequestsTest = 
                new HotRangeRequestsTest(new Range(0, 150),
                    new Range(0, 20),
                    60,
                    42,
                    10_000,
                    CreateSimpleLru(100));

            scenarioRunner.Run(hotRangeRequestsTest, nameof(hotRangeRequestsTest) + "(LRU)");

            hotRangeRequestsTest.Reset(new SimpleCache<int, int>(100));
            scenarioRunner.Run(hotRangeRequestsTest, nameof(hotRangeRequestsTest) + "(Control)");
        }

        private static AdvancedCache<BasicAccessData> CreateSimpleLru(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new SimpleLru());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, maxSize);
            return advancedCache;
        }
    }
}
