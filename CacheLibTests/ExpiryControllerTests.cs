using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CacheLib;
using CacheLib.Expiry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheLibTests
{
    [TestClass]
    public class ExpiryControllerTests
    {
        [TestMethod]
        public void DiscardedAtExpiryTest()
        {
            SimpleCache<string, string> cache = new SimpleCache<string, string>(100);
            using ExpiryController<string, string> expiryController =
                ExpiryController<string, string>.CreateExpiryController(1_000, cache); 

            string A = "A";
            cache.Set(A, A);
            
            Assert.IsTrue(cache.Fetch(A, out string _));

            expiryController.Add(DateTimeOffset.UtcNow.AddSeconds(2), A);

            Assert.IsTrue(cache.Fetch(A, out string _));

            Thread.Sleep(5_000);

            Assert.IsFalse(cache.Fetch(A, out string _));
        }

        [TestMethod]
        public void DiscardedAtUpdatedExpiryTest()
        {
            SimpleCache<string, string> cache = new SimpleCache<string, string>(100);
            using ExpiryController<string, string> expiryController =
                ExpiryController<string, string>.CreateExpiryController(1_000, cache);

            string A = "A";
            cache.Set(A, A);

            Assert.IsTrue(cache.Fetch(A, out string _));
            DateTimeOffset firstExpireTime = DateTimeOffset.UtcNow.AddSeconds(2);

            expiryController.AddOrUpdate(null, firstExpireTime, A);

            Assert.IsTrue(cache.Fetch(A, out string _));

            expiryController.AddOrUpdate(firstExpireTime, DateTimeOffset.UtcNow.AddSeconds(7), A);

            Thread.Sleep(5_000);

            Assert.IsTrue(cache.Fetch(A, out string _));

            Thread.Sleep(5_000);

            Assert.IsFalse(cache.Fetch(A, out string _));
        }
    }
}