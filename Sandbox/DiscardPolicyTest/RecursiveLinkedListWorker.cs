using System.Collections.Generic;
using Sandbox.DiscardPolicyTest.LFRU;
using Sandbox.DiscardPolicyTest.LFU;
using Sandbox.DiscardPolicyTest.LRU;

namespace Sandbox.DiscardPolicyTest
{
    public class RecursiveLinkedListWorker
    {
        public ICollection<LinkedListNode<object>> Lru()
        {
            CacheLib.Discard.DiscardGraph<DataObject> recursiveLinkedList = new CacheLib.Discard.DiscardGraph<DataObject>(new LeastRecentlyUsedPolicy());

            DataObject do1 = new DataObject(1);
            DataObject do2 = new DataObject(2);
            DataObject do3 = new DataObject(3);
            DataObject do4 = new DataObject(4);

            var result1 = recursiveLinkedList.AddNew(do1);
            var result2 = recursiveLinkedList.AddNew(do2);
            var result3 = recursiveLinkedList.AddNew(do3);
            var result4 = recursiveLinkedList.AddNew(do4);

            recursiveLinkedList.UpdateNode(ref result1);
            recursiveLinkedList.UpdateNode(ref result3);

            return recursiveLinkedList.RemoveEntries(4);
        }

        public ICollection<LinkedListNode<object>> Lfu()
        {
            CacheLib.Discard.DiscardGraph<DataObject> recursiveLinkedList = new CacheLib.Discard.DiscardGraph<DataObject>(new LeastFrequentlyUsedPolicy(), new LfuHelperPolicy());

            DataObject do1 = new DataObject(1);
            DataObject do2 = new DataObject(2);
            DataObject do3 = new DataObject(3);
            DataObject do4 = new DataObject(4);

            var result1 = recursiveLinkedList.AddNew(do1);
            var result2 = recursiveLinkedList.AddNew(do2);
            var result3 = recursiveLinkedList.AddNew(do3);
            var result4 = recursiveLinkedList.AddNew(do4);

            do1.OnSomeCommandInvoked(12);
            do3.OnSomeCommandInvoked(16);

            recursiveLinkedList.UpdateNode(ref result1);
            recursiveLinkedList.UpdateNode(ref result3);

            do1.OnSomeCommandInvoked(21);
            recursiveLinkedList.UpdateNode(ref result1);

            return recursiveLinkedList.RemoveEntries(4);
        }

        public ICollection<LinkedListNode<object>> LfRu()
        {
            LfRuTopPolicy topPolicy = new LfRuTopPolicy();
            CacheLib.Discard.DiscardGraph<DataObject> recursiveLinkedList = new CacheLib.Discard.DiscardGraph<DataObject>(topPolicy, new LfRuFrequencyPolicy(), new LfRuLastAccessedPolicy());

            DataObject do1 = new DataObject(1);
            DataObject do2 = new DataObject(2);
            DataObject do3 = new DataObject(3);
            DataObject do4 = new DataObject(4);

            var result1 = recursiveLinkedList.AddNew(do1);
            var result2 = recursiveLinkedList.AddNew(do2);
            var result3 = recursiveLinkedList.AddNew(do3);
            var result4 = recursiveLinkedList.AddNew(do4);

            topPolicy.NewDay = true;
            do1.OnSomeCommandInvoked(12);
            do2.OnSomeCommandInvoked(22);

            recursiveLinkedList.UpdateNode(ref result1);
            recursiveLinkedList.UpdateNode(ref result2);

            return recursiveLinkedList.RemoveEntries(4);
        }
    }
}
