using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib.Discard
{
    internal class RecursiveLinkedList<TValue> : LinkedList<TValue>
    {
        public LinkedListNode<object> ClusterNode { get; private set; }

        public object ClusterData { get; }

        private RecursiveLinkedList(object clusterData)
        {
            ClusterData = clusterData;
        }

        public static RecursiveLinkedList<T> CreateFirst<T>(LinkedList<object> list, object clusterData)
        {
            RecursiveLinkedList<T> awareLinkedList = new RecursiveLinkedList<T>(clusterData);
            awareLinkedList.ClusterNode = list.AddFirst(awareLinkedList);

            return awareLinkedList;
        }

        public static RecursiveLinkedList<T> CreateLast<T>(LinkedList<object> list, object clusterData)
        {
            RecursiveLinkedList<T> awareLinkedList = new RecursiveLinkedList<T>(clusterData);
            awareLinkedList.ClusterNode = list.AddLast(awareLinkedList);

            return awareLinkedList;
        }

        public static RecursiveLinkedList<T> CreateAfter<T>(LinkedList<object> list, object clusterData, LinkedListNode<object> afterThis)
        {
            RecursiveLinkedList<T> awareLinkedList = new RecursiveLinkedList<T>(clusterData);
            awareLinkedList.ClusterNode = list.AddAfter(afterThis, awareLinkedList);

            return awareLinkedList;
        }

        public static RecursiveLinkedList<T> CreateInitial<T>()
        {
            return new RecursiveLinkedList<T>(null);
        }
    }
}
