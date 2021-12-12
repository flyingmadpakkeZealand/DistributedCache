using CacheLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheLibTests
{
    [TestClass]
    public class SimpleCacheTests
    {
        private const int KEY = 4;
        private const int VALUE = 2;

        private SimpleCache<int, int> cache;

        [TestInitialize]
        public void TestInit()
        {
            cache = new SimpleCache<int, int>(10);
            Assert.IsTrue(cache.Set(KEY, VALUE));
        }

        [TestMethod]
        public void Fetch_RetrievesValue()
        {
            bool fetchTrue = cache.Fetch(KEY, out int fetchValue);
            bool fetchFalse = cache.Fetch(0, out int fetchNone);

            Assert.IsTrue(fetchTrue);
            Assert.AreEqual(VALUE, fetchValue);
            Assert.IsFalse(fetchFalse);
            Assert.AreNotEqual(VALUE, fetchNone);
        }

        [TestMethod]
        public void Set_WritesNewEntry()
        {
            int value = 420, key = 69;

            bool setTrue = cache.Set(key, value);
            cache.Fetch(key, out int fetchValue);

            Assert.IsTrue(setTrue);
            Assert.AreEqual(value, fetchValue);
        }

        [TestMethod]
        public void Delete_DeletesEntry()
        {
            bool deleted = cache.Delete(KEY);
            bool deletedAgain = cache.Delete(KEY);

            Assert.IsTrue(deleted);
            Assert.IsFalse(deletedAgain);
        }

        [TestMethod]
        public void Delete_ReturnsOldValue()
        {
            cache.Delete(KEY, out int oldValue);

            Assert.AreEqual(VALUE, oldValue);
        }

        [TestMethod]
        public void CompareAndSwap_AddNewEntry()
        {
            bool newTrue = cache.CompareAndSwap(1, default, 1);
            bool newFalse = cache.CompareAndSwap(1, default, 2);

            Assert.IsTrue(newTrue);
            Assert.IsFalse(newFalse);
        }

        [TestMethod]
        public void CompareAndSwap_UpdateEntry()
        {
            bool updateTrue = cache.CompareAndSwap(KEY, VALUE, 0);
            bool updateFalse = cache.CompareAndSwap(KEY, VALUE, 0);

            Assert.IsTrue(updateTrue);
            Assert.IsFalse(updateFalse);
        }

        [TestMethod]
        public void Set_EntryRemovedOnMaxSizeReached()
        {
            cache = new SimpleCache<int, int>(1);
            Assert.IsTrue(cache.Set(6, 9));
            Assert.IsTrue(cache.Set(2, 2));

            bool fetch1 = cache.Fetch(6, out int _);
            bool fetch2 = cache.Fetch(2, out int _);
            bool canFetchTwo = fetch1 && fetch2;
            bool canFetchOne = fetch1 || fetch2;

            Assert.IsFalse(canFetchTwo);
            Assert.IsTrue(canFetchOne);
        }

        [TestMethod]
        public void CompareAndSwap_EntryRemovedOnMaxSizeReached()
        {
            cache = new SimpleCache<int, int>(1);
            Assert.IsTrue(cache.CompareAndSwap(6, default, 9));
            Assert.IsTrue(cache.CompareAndSwap(2, default, 2));

            bool fetch1 = cache.Fetch(6, out int _);
            bool fetch2 = cache.Fetch(2, out int _);
            bool canFetchTwo = fetch1 && fetch2;
            bool canFetchOne = fetch1 || fetch2;

            Assert.IsFalse(canFetchTwo);
            Assert.IsTrue(canFetchOne);
        }
    }
}