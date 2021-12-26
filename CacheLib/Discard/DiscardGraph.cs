using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib.Discard
{
    public class DiscardGraph<TValue>
    {
        private readonly RecursiveLinkedList<object> _rootDimension;
        private readonly int _totalDimensions;
        private readonly IDiscardPolicy<TValue>[] _policies;

        public DiscardGraph([NotNull] params IDiscardPolicy<TValue>[] policies)
        {
            int dimensions = policies.Length;
            if (dimensions < 1)
                throw new ArgumentOutOfRangeException(nameof(policies));

            _policies = policies;
            _totalDimensions = dimensions;

            _rootDimension = RecursiveLinkedList<object>.CreateInitial<object>();
        }

        public LinkedListNode<object> AddNew([NotNull] TValue listValue)
        {
            return TraverseDownAndAdd(_rootDimension, 1, listValue);
        }

        public void UpdateNode(ref LinkedListNode<object> bottomNode)
        {
            TValue listValue = (TValue)bottomNode.Value;

            Stack<LinkedListNode<object>> clusterNodeStack = GetClusterNodeStack(bottomNode, 1);

            int dimensionLevel = 1;
            IDiscardPolicy<TValue> policy;
            int targetDimensionLevel;
            for (int i = 0; i < _totalDimensions - 1; i++)
            {
                policy = _policies[i];
                LinkedListNode<object> down = clusterNodeStack.Pop();
                RecursiveLinkedList<object> currentCluster = (RecursiveLinkedList<object>)down.Value;

                targetDimensionLevel = policy.Change(currentCluster.ClusterData, listValue);
                if (targetDimensionLevel > 0)
                {
                    bottomNode = TraverseUpAndUpdate(down, dimensionLevel, targetDimensionLevel, bottomNode);
                    clusterNodeStack = GetClusterNodeStack(bottomNode, dimensionLevel + 1);
                }

                dimensionLevel++;
            }

            policy = _policies[_totalDimensions - 1];
            targetDimensionLevel = policy.Change(null, listValue);
            if (targetDimensionLevel > 0)
            {
                bottomNode = TraverseUpAndUpdate(bottomNode, dimensionLevel, targetDimensionLevel, bottomNode);
            }
        }

        private Stack<LinkedListNode<object>> GetClusterNodeStack(LinkedListNode<object> bottomNode, int targetDimension)
        {
            RecursiveLinkedList<object> currentCluster = (RecursiveLinkedList<object>)bottomNode.List;

            Stack<LinkedListNode<object>> clusterNodeStack = new Stack<LinkedListNode<object>>(_totalDimensions);
            LinkedListNode<object> up;

            for (int i = _totalDimensions; i > targetDimension; i--)
            {
                up = currentCluster!.ClusterNode;
                currentCluster = (RecursiveLinkedList<object>)up.List;
                clusterNodeStack.Push(up);
            }

            return clusterNodeStack;
        }

        public ICollection<LinkedListNode<object>> RemoveEntries(int amount = 1)
        {
            ICollection<LinkedListNode<object>> removedNodes = new List<LinkedListNode<object>>();
            GoDown(_rootDimension, removedNodes, amount, 1);

            return removedNodes;
        }

        private void GoHorizontal(RecursiveLinkedList<object> currentCluster, int direction, ICollection<LinkedListNode<object>> removedNodes, int amount, int currentDimensionLevel)
        {
            LinkedListNode<object> currentNode = direction == 0 ? currentCluster.First : currentCluster.Last;

            while (currentNode is not null)
            {
                GoDown((RecursiveLinkedList<object>)currentNode.Value, removedNodes, amount, currentDimensionLevel + 1);
                if (removedNodes.Count == amount) return;
                currentNode = direction == 0 ? currentNode.Next : currentNode.Previous;
            }
        }

        private void GoDown(RecursiveLinkedList<object> currentCluster, ICollection<LinkedListNode<object>> removedNodes, int amount, int currentDimensionLevel)
        {
            IDiscardPolicy<TValue> currentPolicy = _policies[currentDimensionLevel - 1];
            int direction = currentPolicy.Deletion();
            if (currentDimensionLevel == _totalDimensions)
            {
                while (removedNodes.Count < amount)
                {
                    LinkedListNode<object> nodeToRemove = direction == 0 ? currentCluster.First : currentCluster.Last;

                    if (nodeToRemove is null) return;
                    removedNodes.Add(nodeToRemove);
                    currentCluster.Remove(nodeToRemove);
                }

                return;
            }

            GoHorizontal(currentCluster, direction, removedNodes, amount, currentDimensionLevel);
        }

        private LinkedListNode<object> TraverseUpAndUpdate([NotNull] LinkedListNode<object> start, int initialDimensionLevel, int targetDimensionLevel, [NotNull] LinkedListNode<object> bottomNode)
        {
            if (initialDimensionLevel < targetDimensionLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(initialDimensionLevel));
            }

            RecursiveLinkedList<object> currentCluster = (RecursiveLinkedList<object>)start.List;
            RecursiveLinkedList<object> bottomCluster = (RecursiveLinkedList<object>)bottomNode.List;
            bottomCluster!.Remove(bottomNode);
            TValue listValue = (TValue)bottomNode.Value;

            if (targetDimensionLevel == _totalDimensions)
            {
                return TraverseDownAndAdd(currentCluster, targetDimensionLevel, listValue);
            }

            LinkedListNode<object> up = start;
            for (int i = initialDimensionLevel; i > targetDimensionLevel; i--)
            {
                up = currentCluster!.ClusterNode;
                currentCluster = (RecursiveLinkedList<object>)up.List;
            }

            IDiscardPolicy<TValue> policy = _policies[targetDimensionLevel - 1];
            return TraverseRightThenDownAndAdd(up, targetDimensionLevel, listValue, policy.LookAhead); //TODO: Traverse left.
        }

        private LinkedListNode<object> TraverseRightThenDownAndAdd(LinkedListNode<object> start, int currentDimensionLevel,
            TValue listValue, int lookAhead = 1) //TODO: Must return the created bottom node from TraverseDownAndAdd. 
        {
            if (currentDimensionLevel == _totalDimensions)
            {
                throw new ArgumentOutOfRangeException(nameof(currentDimensionLevel));
            }
            //Going Right

            IDiscardPolicy<TValue> policy = _policies[currentDimensionLevel - 1];
            LinkedListNode<object> currentNode = start;

            LinkedListNode<object> next;
            if (lookAhead < 0)
            {
                next = currentNode.List.Last;
                lookAhead = 2;
            }
            else
            {
                next = currentNode.Next;
            }

            for (int i = 0; i < lookAhead; i++)
            {
                if (next is null)
                {
                    RecursiveLinkedList<object> newLastCluster =
                        RecursiveLinkedList<object>.CreateLast<object>(currentNode.List, policy.ClusterData(listValue));

                    return TraverseDownAndAdd(newLastCluster, currentDimensionLevel + 1, listValue);
                }

                RecursiveLinkedList<object> awareLinkedList = (RecursiveLinkedList<object>)next.Value;
                if (i == lookAhead - 1 || policy.Change(awareLinkedList.ClusterData, listValue) < 1)
                {
                    if (policy.Allowance(awareLinkedList.ClusterData, listValue))
                    {
                        return TraverseDownAndAdd(awareLinkedList, currentDimensionLevel + 1, listValue);
                    }

                    RecursiveLinkedList<object> newlyCreated = RecursiveLinkedList<object>.CreateAfter<object>(currentNode.List, policy.ClusterData(listValue),
                        currentNode);

                    return TraverseDownAndAdd(newlyCreated, currentDimensionLevel + 1, listValue);
                }

                currentNode = next;
                next = currentNode.Next;
            }

            throw new ArgumentOutOfRangeException(nameof(lookAhead));
        }

        private LinkedListNode<object> TraverseDownAndAdd(RecursiveLinkedList<object> start, int currentDimensionLevel, TValue listValue)
        {
            RecursiveLinkedList<object> currentCluster = start;
            IDiscardPolicy<TValue> currentPolicy = _policies[currentDimensionLevel - 1];
            int position = currentPolicy.Insertion(listValue);

            for (int i = currentDimensionLevel; i < _totalDimensions; i++)
            {
                RecursiveLinkedList<object> nextCluster;
                if (position == 0)
                {
                    LinkedListNode<object> first = currentCluster.First;
                    if (first is null)
                    {
                        nextCluster = RecursiveLinkedList<object>.CreateFirst<object>(currentCluster,
                            currentPolicy.ClusterData(listValue));
                    }
                    else
                    {
                        RecursiveLinkedList<object> awareLinkedList = (RecursiveLinkedList<object>)first.Value;
                        if (currentPolicy.Allowance(awareLinkedList.ClusterData, listValue))
                        {
                            nextCluster = awareLinkedList;
                        }
                        else
                        {
                            nextCluster = RecursiveLinkedList<object>.CreateFirst<object>(currentCluster,
                                currentPolicy.ClusterData(listValue));
                        }
                    }
                }
                else
                {
                    LinkedListNode<object> last = currentCluster.Last;
                    if (last is null)
                    {
                        nextCluster = RecursiveLinkedList<object>.CreateLast<object>(currentCluster,
                            currentPolicy.ClusterData(listValue));
                    }
                    else
                    {
                        RecursiveLinkedList<object> awareLinkedList = (RecursiveLinkedList<object>)last.Value;
                        if (currentPolicy.Change(awareLinkedList.ClusterData, listValue) > 0)
                        {
                            nextCluster = RecursiveLinkedList<object>.CreateLast<object>(currentCluster,
                                currentPolicy.ClusterData(listValue));
                        }
                        else
                        {
                            nextCluster = awareLinkedList;
                        }
                    }
                }

                currentPolicy = _policies[i];
                position = currentPolicy.Insertion(listValue);
                currentCluster = nextCluster;
            }

            return position == 0 ? currentCluster.AddFirst(listValue) : currentCluster.AddLast(listValue);
        }
    }
}
