using CacheLib.Discard;

namespace Sandbox.DiscardPolicyTest.LFRU
{
    public class LfRuFrequencyPolicy : IDiscardPolicy<DataObject>
    {
        public int LookAhead { get; } = 10;

        public int Insertion(DataObject listValue)
        {
            return 0;
        }

        public int Deletion()
        {
            return 0;
        }

        public object ClusterData(DataObject listValue)
        {
            DataObject dataObject = listValue;

            return 10 * (dataObject.HitCount / 10);
        }

        public int Change(object clusterData, DataObject listValue)
        {
            int count = (int)clusterData;
            DataObject dataObject = listValue;

            if (dataObject.HitCount >= count + 10 && dataObject.HitCount <= 50)
            {
                return 2;
            }

            return -1;
        }

        public bool Allowance(object clusterData, DataObject listValue)
        {
            int count = (int)clusterData;
            DataObject dataObject = listValue;

            return dataObject.HitCount >= count;
        }
    }
}
