using CacheLib.Discard;

namespace Sandbox.DiscardPolicyTest.LFRU
{
    public class LfRuTopPolicy : IDiscardPolicy<DataObject>
    {
        public bool NewDay { get; set; }

        public int LookAhead { get; } = -1;

        public int Insertion(DataObject listValue)
        {
            return 1;
        }

        public int Deletion()
        {
            return 0;
        }

        public object ClusterData(DataObject listValue)
        {
            return null;
        }

        public int Change(object clusterData, DataObject listValue)
        {
            return NewDay ? 1 : -1;
        }

        public bool Allowance(object clusterData, DataObject listValue)
        {
            return true;
        }
    }
}
