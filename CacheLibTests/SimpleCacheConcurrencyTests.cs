using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheLibTests
{
    [TestClass]
    public class SimpleCacheConcurrencyTests
    {
        private SimpleCache<int, int> cache;
        private readonly Random _rand = new Random(42);
        private readonly object _lock = new object();

        //[TestInitialize]
        //public void Init()
        //{
        //    cache = new SimpleCache<int, int>(10);
        //}

        [TestMethod]
        public void Delete_NoRace()
        {
            for (int i = 0; i < 1_000; i++)
            {
                cache = new SimpleCache<int, int>(10);
                cache.Set(4, 2);

                int valuesRemoved = StartConcurrentTaskAndGetResult(10,
                        () => cache.Delete(4))
                    .Count(b => b);

                Assert.AreEqual(1, valuesRemoved);
            }
        }

        [TestMethod]
        public void CompareAndSwap_AddOneNewEntry_NoRace()
        {
            for (int i = 0; i < 1_000; i++)
            {
                cache = new SimpleCache<int, int>(10);

                int valuesAdded = StartConcurrentTaskAndGetResult(10,
                        () => cache.CompareAndSwap(4, default, 2))
                    .Count(b => b);

                Assert.AreEqual(1, valuesAdded);
            }
        }

         //Also proves Async logging algorithm.
        [TestMethod]
        public void Set_ReturnsOldValue_NoRace_AsyncAlgorithmProof()
        {
            for (int i = 0; i < 1_000; i++)
            {
                cache = new SimpleCache<int, int>( 10);
                ValueTuple<int, int> readWrite0 = (0, 1);
                ValueTuple<int, int> readWrite1 = (0, 0);
                ValueTuple<int, int> readWrite2 = (0, 0);

                var result = StartConcurrentTaskAndGetResult(10,
                    () => SetRandomNumberBetween0And2(cache));

                foreach ((int read, int write) readWrite in result)
                {
                    (int read, int write) = readWrite;

                    switch (read)
                    {
                        case 0: readWrite0.Item1++; break;
                        case 1: readWrite1.Item1++; break;
                        case 2: readWrite2.Item1++; break;
                    }

                    switch (write)
                    {
                        case 0: readWrite0.Item2++; break;
                        case 1: readWrite1.Item2++; break;
                        case 2: readWrite2.Item2++; break;
                    }
                    
                }

                int? finalState = null;
                if (readWrite0.Item1 != readWrite0.Item2)
                {
                    finalState = 0;
                }

                if (readWrite1.Item1 != readWrite1.Item2)
                {
                    if (finalState is not null)
                    {
                        Assert.Fail();
                    }

                    finalState = 1;
                }

                if (readWrite2.Item1 != readWrite2.Item2)
                {
                    if (finalState is not null)
                    {
                        Assert.Fail();
                    }

                    finalState = 2;
                }

                if (finalState is null)
                {
                    Assert.Fail();
                }
                else
                {
                    cache.Fetch(42, out int actualFinalState);

                    Assert.AreEqual((int) finalState, actualFinalState);
                }
            }
        }

        private (int read, int write) SetRandomNumberBetween0And2(SimpleCache<int, int> cache)
        {
            int number = GetNumber();
            cache.Set(42, number, out int oldValue);

            return (oldValue, number);
        }

        private int GetNumber()
        {
            lock (_lock)
            {
                return _rand.Next(3);
            }
        }

        private IEnumerable<TOut> StartConcurrentTaskAndGetResult<TOut>(int concurrency, Func<TOut> taskFunc)
        {
            Task<TOut>[] concurrentJobs = new Task<TOut>[concurrency];

            for (int i = 0; i < concurrency; i++)
            {
                concurrentJobs[i] = new Task<TOut>(taskFunc);
            }

            for (int i = 0; i < concurrency; i++)
            {
                concurrentJobs[i].Start();
            }

            Task.WaitAll(concurrentJobs);

            return concurrentJobs.Select(task => task.Result);
        }
    }
}
