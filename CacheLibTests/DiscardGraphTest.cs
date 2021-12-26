using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib.Discard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheLibTests
{
    [TestClass]
    public class DiscardGraphTest
    {
        private const string A = "A";
        private const string B = "B";
        private const string C = "C";
        private const string D = "D";

        [TestMethod]
        public void SimpleLruTest()
        {
            DiscardGraph<string> discardGraph = new DiscardGraph<string>(new SimpleLru());
            discardGraph.AddNew(A);
            discardGraph.AddNew(B);
            discardGraph.AddNew(C);
            discardGraph.AddNew(D);

            ICollection<LinkedListNode<object>> removed = discardGraph.RemoveEntries(4);
            LinkedListNode<object>[] array = removed.ToArray();

            Assert.AreEqual(array[0].Value, A);
            Assert.AreEqual(array[1].Value, B);
            Assert.AreEqual(array[2].Value, C);
            Assert.AreEqual(array[3].Value, D);

            var result1 = discardGraph.AddNew(A);
            var result2 = discardGraph.AddNew(B);
            var result3 = discardGraph.AddNew(C);
            var result4 = discardGraph.AddNew(D);

            discardGraph.UpdateNode(ref result1);
            discardGraph.UpdateNode(ref result2);

            removed = discardGraph.RemoveEntries(4);
            array = removed.ToArray();

            Assert.AreEqual(array[0].Value, C);
            Assert.AreEqual(array[1].Value, D);
            Assert.AreEqual(array[2].Value, A);
            Assert.AreEqual(array[3].Value, B);
        }

        [TestMethod]
        public void SimpleLfuTest()
        {
            Data dA = new Data(1, A);
            Data dB = new Data(2, B);
            Data dC = new Data(3, C);
            Data dD = new Data(4, D);

            DiscardGraph<Data> discardGraph = new DiscardGraph<Data>(new SimpleOrderByFrequency(), new SimpleLru2());
            var result1 = discardGraph.AddNew(dA);
            var result2 = discardGraph.AddNew(dB);
            var result3 = discardGraph.AddNew(dC);
            var result4 = discardGraph.AddNew(dD);

            ICollection<LinkedListNode<object>> removed = discardGraph.RemoveEntries(4);
            LinkedListNode<object>[] array = removed.ToArray();

            Assert.AreEqual(array[0].Value, dA);
            Assert.AreEqual(array[1].Value, dB);
            Assert.AreEqual(array[2].Value, dC);
            Assert.AreEqual(array[3].Value, dD);

            result1 = discardGraph.AddNew(dA);
            result2 = discardGraph.AddNew(dB);
            result3 = discardGraph.AddNew(dC);
            result4 = discardGraph.AddNew(dD);

            dA.HitCount = 32;
            dC.HitCount = 12;

            discardGraph.UpdateNode(ref result1);
            discardGraph.UpdateNode(ref result3);

            removed = discardGraph.RemoveEntries(4);
            array = removed.ToArray();

            Assert.AreEqual(array[0].Value, dB);
            Assert.AreEqual(array[1].Value, dD);
            Assert.AreEqual(array[2].Value, dC);
            Assert.AreEqual(array[3].Value, dA);
        }

        private class Data
        {
            public string Str { get; private set; }
            public int HitCount { get; set; }

            public Data(int hitCount, string str)
            {
                Str = str;
                HitCount = hitCount;
            }
        }

        private class SimpleOrderByFrequency : IDiscardPolicy<Data>
        {
            public int LookAhead { get; } = 5;

            public int Insertion(Data listValue)
            {
                return 0;
            }

            public int Deletion()
            {
                return 0;
            }

            public object ClusterData(Data listValue)
            {
                return 10 * (listValue.HitCount / 10);
            }

            public int Change(object clusterData, Data listValue)
            {
                int count = (int) clusterData;

                if (listValue.HitCount >= count + 10 && listValue.HitCount < 60)
                {
                    return 1;
                }

                return 0;
            }

            public bool Allowance(object clusterData, Data listValue)
            {
                int count = (int) clusterData;

                return listValue.HitCount >= count;
            }
        }

        private class SimpleLru2 : IDiscardPolicy<Data>
        {
            public int LookAhead { get; } = 1;

            public int Insertion(Data listValue)
            {
                return 0;
            }

            public int Deletion()
            {
                return 1;
            }

            public object ClusterData(Data listValue)
            {
                return null;
            }

            public int Change(object clusterData, Data listValue)
            {
                return 2;
            }

            public bool Allowance(object clusterData, Data listValue)
            {
                return true;
            }
        }

        private class SimpleLru : IDiscardPolicy<string>
        {
            public int LookAhead { get; } = 1;

            public int Insertion(string listValue)
            {
                return 0;
            }

            public int Deletion()
            {
                return 1;
            }

            public object ClusterData(string listValue)
            {
                return null;
            }

            public int Change(object clusterData, string listValue)
            {
                return 1;
            }

            public bool Allowance(object clusterData, string listValue)
            {
                return true;
            }
        }
    }
}
