using CacheLib.Discard;

namespace Sandbox.DiscardPolicyTest.LRU
{
    public class LeastRecentlyUsedPolicy : IDiscardPolicy<DataObject>
    {
        public int LookAhead { get; } = 1;

        public int Insertion(DataObject listValue)
        {
            return 0;
        }

        public int Deletion()
        {
            return 1;
        }

        public object ClusterData(DataObject listValue)
        {
            return null;
        }

        public int Change(object clusterData, DataObject listValue)
        {
            return 1;
        }

        public bool Allowance(object clusterData, DataObject listValue)
        {
            return true;
        }
    }
}
