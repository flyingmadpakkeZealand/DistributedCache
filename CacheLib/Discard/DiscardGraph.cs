using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib.Discard
{
    public class DiscardGraph<TValue>
    {
        private readonly RecursiveLinkedList _rootDimension;
        private readonly int _totalDimensions;
        private readonly IDiscardPolicy<TValue>[] _policies;

        public DiscardGraph([NotNull] params IDiscardPolicy<TValue>[] policies)
        {
            int dimensionsCount = policies.Length;
            if (dimensionsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(policies));

            _policies = new IDiscardPolicy<TValue>[dimensionsCount];

            foreach (IDiscardPolicy<TValue> policy in policies)
            {
                int dimension = policy.ThisDimension;

                if (dimension <= 0 || dimension > dimensionsCount) throw new ArgumentOutOfRangeException(nameof(policy.ThisDimension));

                if (_policies[dimension - 1] is not null)
                {
                    IDiscardPolicy<TValue> policy2 = _policies[dimension - 1];

                    throw new AmbiguousMatchException($"Two policies ('{policy2.GetType().FullName}' AND '{policy.GetType().FullName}') has equal dimension '{dimension}', each policy must have a different dimension.");
                }

                _policies[dimension - 1] = policy;
            }

            _totalDimensions = dimensionsCount;

            _rootDimension = RecursiveLinkedList.CreateInitial();
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
                RecursiveLinkedList currentCluster = (RecursiveLinkedList)down.Value;

                targetDimensionLevel = policy.ChangeTo(currentCluster.ClusterData, clusterValue);
                if (targetDimensionLevel > 0)
                {
                    bottomNode = TraverseUpAndUpdate(down, dimensionLevel, targetDimensionLevel, bottomNode);
                    clusterNodeStack = GetClusterNodeStack(bottomNode, dimensionLevel + 1);
                }

                dimensionLevel++;
            }

            policy = _policies[_totalDimensions - 1];
            targetDimensionLevel = policy.ChangeTo(null, clusterValue);
            if (targetDimensionLevel > 0)
            {
                bottomNode = TraverseUpAndUpdate(bottomNode, dimensionLevel, targetDimensionLevel, bottomNode);
            }
        }

        private Stack<LinkedListNode<object>> GetClusterNodeStack(LinkedListNode<object> bottomNode, int targetDimension)
        {
            RecursiveLinkedList currentCluster = (RecursiveLinkedList)bottomNode.List;

            Stack<LinkedListNode<object>> clusterNodeStack = new Stack<LinkedListNode<object>>(_totalDimensions);

            for (int i = _totalDimensions; i > targetDimension; i--)
            {
                LinkedListNode<object> up = currentCluster!.ClusterNode;
                currentCluster = (RecursiveLinkedList)up.List;
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

        private void GoHorizontal(RecursiveLinkedList currentCluster, int direction, ICollection<LinkedListNode<object>> removedNodes, int amount, int currentDimensionLevel)
        {
            LinkedListNode<object> currentNode = direction == 0 ? currentCluster.First : currentCluster.Last;

            while (currentNode is not null)
            {
                GoDownAndRemove((RecursiveLinkedList)currentNode.Value, removedNodes, amount, currentDimensionLevel + 1);
                if (removedNodes.Count == amount) return;
                currentNode = direction == 0 ? currentNode.Next : currentNode.Previous;
            }
        }

        private void GoDownAndRemove(RecursiveLinkedList currentCluster, ICollection<LinkedListNode<object>> removedNodes, int amount, int currentDimensionLevel)
        {
            IDiscardPolicy<TValue> currentPolicy = _policies[currentDimensionLevel - 1];
            int direction = (int) currentPolicy.Deletion();
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

            RecursiveLinkedList currentCluster = (RecursiveLinkedList)start.List;
            RecursiveLinkedList bottomCluster = (RecursiveLinkedList)bottomNode.List;
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
                currentCluster = (RecursiveLinkedList)up.List;
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
                    RecursiveLinkedList newLastCluster =
                        RecursiveLinkedList.CreateLast(currentNode.List, policy.ClusterData(clusterValue));

                    return TraverseDownAndAdd(newLastCluster, currentDimensionLevel + 1, clusterValue);
                }

                RecursiveLinkedList nextCluster = (RecursiveLinkedList)next.Value;
                if (i == lookAhead - 1 || policy.ChangeTo(nextCluster.ClusterData, clusterValue) < 1)
                {
                    if (policy.Allowance(nextCluster.ClusterData, clusterValue))
                    {
                        return TraverseDownAndAdd(nextCluster, currentDimensionLevel + 1, clusterValue);
                    }

                    RecursiveLinkedList newlyCreatedCluster = RecursiveLinkedList.CreateAfter(currentNode.List, policy.ClusterData(clusterValue),
                        currentNode);

                    return TraverseDownAndAdd(newlyCreatedCluster, currentDimensionLevel + 1, clusterValue);
                }

                currentNode = next;
                next = currentNode.Next;
            }

            throw new ArgumentOutOfRangeException(nameof(lookAhead));
        }

        private LinkedListNode<object> TraverseDownAndAdd(RecursiveLinkedList start, int currentDimensionLevel, TValue clusterValue)
        {
            RecursiveLinkedList currentCluster = start;
            IDiscardPolicy<TValue> currentPolicy = _policies[currentDimensionLevel - 1];
            ClusterPosition position = currentPolicy.Insertion(clusterValue);

            for (int i = currentDimensionLevel; i < _totalDimensions; i++)
            {
                RecursiveLinkedList nextCluster;
                if (position == ClusterPosition.First)
                {
                    LinkedListNode<object> first = currentCluster.First;
                    if (first is null)
                    {
                        nextCluster = RecursiveLinkedList.CreateFirst(currentCluster,
                            currentPolicy.ClusterData(clusterValue));
                    }
                    else
                    {
                        RecursiveLinkedList firstCluster = (RecursiveLinkedList)first.Value;
                        if (currentPolicy.Allowance(firstCluster.ClusterData, clusterValue))
                        {
                            nextCluster = firstCluster;
                        }
                        else
                        {
                            nextCluster = RecursiveLinkedList.CreateFirst(currentCluster,
                                currentPolicy.ClusterData(clusterValue));
                        }
                    }
                }
                else
                {
                    LinkedListNode<object> last = currentCluster.Last;
                    if (last is null)
                    {
                        nextCluster = RecursiveLinkedList.CreateLast(currentCluster,
                            currentPolicy.ClusterData(clusterValue));
                    }
                    else
                    {
                        RecursiveLinkedList lastCluster = (RecursiveLinkedList)last.Value;
                        if (currentPolicy.ChangeTo(lastCluster.ClusterData, clusterValue) > 0)
                        {
                            nextCluster = RecursiveLinkedList.CreateLast(currentCluster,
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

            return position == ClusterPosition.First ? currentCluster.AddFirst(clusterValue) : currentCluster.AddLast(clusterValue);
        }
    }
}
