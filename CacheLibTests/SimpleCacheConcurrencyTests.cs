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
