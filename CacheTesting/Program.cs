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

            randomRequestsTest.Reset(CreateSimpleLfRu(100));
            scenarioRunner.Run(randomRequestsTest, nameof(randomRequestsTest) + "(LFRU)");

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

            hotRangeRequestsTest.Reset(CreateRandLfRu(100));
            scenarioRunner.Run(hotRangeRequestsTest, nameof(hotRangeRequestsTest) + "(LFRU)");

            //hotRangeRequestsTest.Reset(CreateProbabilityLfu(100));
            //scenarioRunner.Run(hotRangeRequestsTest, nameof(hotRangeRequestsTest) + "(ProbLFU)");

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

            cyclicRequestsTest.Reset(CreateSimpleLfRu(100));
            scenarioRunner.Run(cyclicRequestsTest, nameof(cyclicRequestsTest) + "(LFRU)");

            //cyclicRequestsTest.Reset(CreateProbabilityLfu(100));
            //scenarioRunner.Run(cyclicRequestsTest, nameof(cyclicRequestsTest) + "(ProbLFU)");

            cyclicRequestsTest.Reset(new SimpleCache<int, int>(100));
            scenarioRunner.Run(cyclicRequestsTest, nameof(cyclicRequestsTest) + "(Control)");

            PhaseRequestsTest phaseRequestsTest = 
                new PhaseRequestsTest(42, 
                    100_000,
                    CreateSimpleLru(100),
                    10,
                    new Range(0, 9));

            //scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(LRU)");

            phaseRequestsTest.Reset(CreateSimpleLfu(100));
            scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(LFU)");

            //phaseRequestsTest.Reset(CreateProbabilityLfu(100));
            //scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(ProbLFU)");

            //phaseRequestsTest.Reset(CreateRandLfu(100));
            //scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(RandLFU)");

            phaseRequestsTest.Reset(CreateProbRandLfu(100));
            scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(RandProbLFU)");

            phaseRequestsTest.Reset(CreateSimpleLfRu(100));
            scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(LFRU)");

            //phaseRequestsTest.Reset(CreateProbabilityLfRu(100));
            //scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(ProbLFRU)");

            //phaseRequestsTest.Reset(CreateRandLfRu(100));
            //scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(RandLFRU");

            phaseRequestsTest.Reset(CreateProbRandLfRu(100));
            scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(RandProbLFRU)");

            phaseRequestsTest.Reset(new SimpleCache<int, int>(100));
            scenarioRunner.Run(phaseRequestsTest, nameof(phaseRequestsTest) + "(Control)");
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

        private static AdvancedCache<BasicAccessData> CreateProbabilityLfu(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new ProbabilityLfu(), new ProbabilityLfuBottomLru());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, maxSize);
            return advancedCache;
        }

        private static AdvancedCache<BasicAccessData> CreateSimpleLfRu(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new SimpleLfRu(), new SimpleLfRuMiddleLfu(), new SimpleLfRuBottomLru());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, maxSize);
            return advancedCache;
        }

        private static AdvancedCache<BasicAccessData> CreateProbabilityLfRu(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new ProbabilityLfRu(), new ProbabilityLfRuMiddleProbLfu(), new ProbabilityLfRuBottomLru());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, 100);
            return advancedCache;
        }

        private static AdvancedCache<BasicAccessData> CreateRandLfRu(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new SimpleLfRu(), new SimpleLfRuMiddleLfu(), new RandomPol());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, maxSize);
            return advancedCache;
        }

        private static AdvancedCache<BasicAccessData> CreateProbRandLfRu(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new ProbabilityLfRu(), new ProbabilityLfRuMiddleProbLfu(), new RandomPol());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, maxSize);
            return advancedCache;
        }

        private static AdvancedCache<BasicAccessData> CreateProbRandLfu(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new ProbabilityLfu(), new ProbabilityLfuBottomRand());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, maxSize);
            return advancedCache;
        }

        private static AdvancedCache<BasicAccessData> CreateRandLfu(int maxSize)
        {
            DiscardGraph<BasicAccessData> discardGraph = new DiscardGraph<BasicAccessData>(new SimpleLfu(), new ProbabilityLfuBottomRand());
            AdvancedCache<BasicAccessData> advancedCache = new AdvancedCache<BasicAccessData>(discardGraph, maxSize);
            return advancedCache;
        }
    }
}
