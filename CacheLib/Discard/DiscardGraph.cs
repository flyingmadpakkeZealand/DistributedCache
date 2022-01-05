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

        public LinkedListNode<object> AddNew([NotNull] TValue clusterValue)
        {
            return TraverseDownAndAdd(_rootDimension, 1, clusterValue);
        }

        public void UpdateNode(ref LinkedListNode<object> bottomNode)
        {
            TValue clusterValue = (TValue)bottomNode.Value;

            Stack<LinkedListNode<object>> clusterNodeStack = GetClusterNodeStack(bottomNode, 1);

            int dimensionLevel = 1;
            IDiscardPolicy<TValue> policy;
            int targetDimensionLevel;
            for (int i = 0; i < _totalDimensions - 1; i++)
            {
                policy = _policies[i];
                LinkedListNode<object> down = clusterNodeStack.Pop();
                RecursiveLinkedList<object> currentCluster = (RecursiveLinkedList<object>)down.Value;

                targetDimensionLevel = policy.Change(currentCluster.ClusterData, clusterValue);
                if (targetDimensionLevel > 0)
                {
                    bottomNode = TraverseUpAndUpdate(down, dimensionLevel, targetDimensionLevel, bottomNode);
                    clusterNodeStack = GetClusterNodeStack(bottomNode, dimensionLevel + 1);
                }

                dimensionLevel++;
            }

            policy = _policies[_totalDimensions - 1];
            targetDimensionLevel = policy.Change(null, clusterValue);
            if (targetDimensionLevel > 0)
            {
                bottomNode = TraverseUpAndUpdate(bottomNode, dimensionLevel, targetDimensionLevel, bottomNode);
            }
        }

        private Stack<LinkedListNode<object>> GetClusterNodeStack(LinkedListNode<object> bottomNode, int targetDimension)
        {
            RecursiveLinkedList<object> currentCluster = (RecursiveLinkedList<object>)bottomNode.List;

            Stack<LinkedListNode<object>> clusterNodeStack = new Stack<LinkedListNode<object>>(_totalDimensions);

            for (int i = _totalDimensions; i > targetDimension; i--)
            {
                LinkedListNode<object> up = currentCluster!.ClusterNode;
                currentCluster = (RecursiveLinkedList<object>)up.List;
                clusterNodeStack.Push(up);
            }

            return clusterNodeStack;
        }

        public ICollection<LinkedListNode<object>> RemoveEntries(int amount = 1)
        {
            ICollection<LinkedListNode<object>> removedNodes = new List<LinkedListNode<object>>();
            GoDownAndRemove(_rootDimension, removedNodes, amount, 1);

            return removedNodes;
        }

        private void GoHorizontal(RecursiveLinkedList<object> currentCluster, int direction, ICollection<LinkedListNode<object>> removedNodes, int amount, int currentDimensionLevel)
        {
            LinkedListNode<object> currentNode = direction == 0 ? currentCluster.First : currentCluster.Last;

            while (currentNode is not null)
            {
                GoDownAndRemove((RecursiveLinkedList<object>)currentNode.Value, removedNodes, amount, currentDimensionLevel + 1);
                if (removedNodes.Count == amount) return;
                currentNode = direction == 0 ? currentNode.Next : currentNode.Previous;
            }
        }

        private void GoDownAndRemove(RecursiveLinkedList<object> currentCluster, ICollection<LinkedListNode<object>> removedNodes, int amount, int currentDimensionLevel)
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
            TValue clusterValue = (TValue)bottomNode.Value;

            if (targetDimensionLevel == _totalDimensions)
            {
                return TraverseDownAndAdd(currentCluster, targetDimensionLevel, clusterValue);
            }

            LinkedListNode<object> up = start;
            for (int i = initialDimensionLevel; i > targetDimensionLevel; i--)
            {
                up = currentCluster!.ClusterNode;
                currentCluster = (RecursiveLinkedList<object>)up.List;
            }

            IDiscardPolicy<TValue> policy = _policies[targetDimensionLevel - 1];
            return TraverseRightThenDownAndAdd(up, targetDimensionLevel, clusterValue, policy.LookAhead); //TODO: Traverse left.
        }

        private LinkedListNode<object> TraverseRightThenDownAndAdd(LinkedListNode<object> start, int currentDimensionLevel,
            TValue clusterValue, int lookAhead = 1) //TODO: Must return the created bottom node from TraverseDownAndAdd. 
        {
            if (currentDimensionLevel >= _totalDimensions)
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
                        RecursiveLinkedList<object>.CreateLast<object>(currentNode.List, policy.ClusterData(clusterValue));

                    return TraverseDownAndAdd(newLastCluster, currentDimensionLevel + 1, clusterValue);
                }

                RecursiveLinkedList<object> nextCluster = (RecursiveLinkedList<object>)next.Value;
                if (i == lookAhead - 1 || policy.Change(nextCluster.ClusterData, clusterValue) < 1)
                {
                    if (policy.Allowance(nextCluster.ClusterData, clusterValue))
                    {
                        return TraverseDownAndAdd(nextCluster, currentDimensionLevel + 1, clusterValue);
                    }

                    RecursiveLinkedList<object> newlyCreatedCluster = RecursiveLinkedList<object>.CreateAfter<object>(currentNode.List, policy.ClusterData(clusterValue),
                        currentNode);

                    return TraverseDownAndAdd(newlyCreatedCluster, currentDimensionLevel + 1, clusterValue);
                }

                currentNode = next;
                next = currentNode.Next;
            }

            throw new ArgumentOutOfRangeException(nameof(lookAhead));
        }

        private LinkedListNode<object> TraverseDownAndAdd(RecursiveLinkedList<object> start, int currentDimensionLevel, TValue clusterValue)
        {
            RecursiveLinkedList<object> currentCluster = start;
            IDiscardPolicy<TValue> currentPolicy = _policies[currentDimensionLevel - 1];
            int position = currentPolicy.Insertion(clusterValue);

            for (int i = currentDimensionLevel; i < _totalDimensions; i++)
            {
                RecursiveLinkedList<object> nextCluster;
                if (position == 0)
                {
                    LinkedListNode<object> first = currentCluster.First;
                    if (first is null)
                    {
                        nextCluster = RecursiveLinkedList<object>.CreateFirst<object>(currentCluster,
                            currentPolicy.ClusterData(clusterValue));
                    }
                    else
                    {
                        RecursiveLinkedList<object> firstCluster = (RecursiveLinkedList<object>)first.Value;
                        if (currentPolicy.Allowance(firstCluster.ClusterData, clusterValue))
                        {
                            nextCluster = firstCluster;
                        }
                        else
                        {
                            nextCluster = RecursiveLinkedList<object>.CreateFirst<object>(currentCluster,
                                currentPolicy.ClusterData(clusterValue));
                        }
                    }
                }
                else
                {
                    LinkedListNode<object> last = currentCluster.Last;
                    if (last is null)
                    {
                        nextCluster = RecursiveLinkedList<object>.CreateLast<object>(currentCluster,
                            currentPolicy.ClusterData(clusterValue));
                    }
                    else
                    {
                        RecursiveLinkedList<object> lastCluster = (RecursiveLinkedList<object>)last.Value;
                        if (currentPolicy.Change(lastCluster.ClusterData, clusterValue) > 0)
                        {
                            nextCluster = RecursiveLinkedList<object>.CreateLast<object>(currentCluster,
                                currentPolicy.ClusterData(clusterValue));
                        }
                        else
                        {
                            nextCluster = lastCluster;
                        }
                    }
                }

                currentPolicy = _policies[i];
                position = currentPolicy.Insertion(clusterValue);
                currentCluster = nextCluster;
            }

            return position == 0 ? currentCluster.AddFirst(clusterValue) : currentCluster.AddLast(clusterValue);
        }
    }
}
