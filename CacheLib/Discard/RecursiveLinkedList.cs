using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib.Discard
{
    internal class RecursiveLinkedList : LinkedList<object>
    {
        public LinkedListNode<object> ClusterNode { get; private set; }

        public object ClusterData { get; }

        private RecursiveLinkedList(object clusterData)
        {
            ClusterData = clusterData;
        }

        public static RecursiveLinkedList CreateFirst(LinkedList<object> list, object clusterData)
        {
            RecursiveLinkedList recursiveLinkedList = new RecursiveLinkedList(clusterData);
            recursiveLinkedList.ClusterNode = list.AddFirst(recursiveLinkedList);

            return recursiveLinkedList;
        }

        public static RecursiveLinkedList CreateLast(LinkedList<object> list, object clusterData)
        {
            RecursiveLinkedList recursiveLinkedList = new RecursiveLinkedList(clusterData);
            recursiveLinkedList.ClusterNode = list.AddLast(recursiveLinkedList);

            return recursiveLinkedList;
        }

        public static RecursiveLinkedList CreateAfter(LinkedList<object> list, object clusterData,
            LinkedListNode<object> afterThis)
        {
            RecursiveLinkedList recursiveLinkedList = new RecursiveLinkedList(clusterData);
            recursiveLinkedList.ClusterNode = list.AddAfter(afterThis, recursiveLinkedList);

            return recursiveLinkedList;
        }

        public static RecursiveLinkedList CreateInitial()
        {
            return new RecursiveLinkedList(null);
        }
    }
}
