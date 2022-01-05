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
                    100_000,
                    CreateSimpleLru(100));
            
            scenarioRunner.Run(randomRequestsTest, nameof(randomRequestsTest)+ "(LRU)");

            randomRequestsTest.Reset(CreateSimpleLfu(100));
            scenarioRunner.Run(randomRequestsTest, nameof(randomRequestsTest) + "(LFU)");

            randomRequestsTest.Reset(new SimpleCache<int, int>(100));
            scenarioRunner.Run(randomRequestsTest, nameof(randomRequestsTest) + "(Control)");

            HotRangeRequestsTest hotRangeRequestsTest = 
                new HotRangeRequestsTest(new Range(0, 150),
                    new Range(0, 20),
                    60,
                    42,
                    100_000,
                    CreateSimpleLru(100));

            scenarioRunner.Run(hotRangeRequestsTest, nameof(hotRangeRequestsTest) + "(LRU)");

            hotRangeRequestsTest.Reset(CreateSimpleLfu(100));
            scenarioRunner.Run(hotRangeRequestsTest, nameof(hotRangeRequestsTest) + "(LFU)");

            hotRangeRequestsTest.Reset(new SimpleCache<int, int>(100));
            scenarioRunner.Run(hotRangeRequestsTest, nameof(hotRangeRequestsTest) + "(Control)");

            CyclicRequestsTest cyclicRequestsTest =
                new CyclicRequestsTest(new Range(0, 150), 
                    new Range(0, 20), 
                    new Range(100, 120), 
                    60,
                    42,
                    100_000,
                    CreateSimpleLru(100));

            scenarioRunner.Run(cyclicRequestsTest, nameof(cyclicRequestsTest) + "(LRU)");

            cyclicRequestsTest.Reset(CreateSimpleLfu(100));
            scenarioRunner.Run(cyclicRequestsTest, nameof(cyclicRequestsTest) + "(LFU)");

            cyclicRequestsTest.Reset(new SimpleCache<int, int>(100));
            scenarioRunner.Run(cyclicRequestsTest, nameof(cyclicRequestsTest) + "(Control)");
        }

        private static AdvancedCache<BasicAccessData> CreateSimpleLru(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new SimpleLru());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, maxSize);
            return advancedCache;
        }

        private static AdvancedCache<BasicAccessData> CreateSimpleLfu(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new SimpleLfu(), new SimpleLfuBottomLru());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, maxSize);
            return advancedCache;
        }
    }
}
