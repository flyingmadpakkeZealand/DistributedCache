using CacheLib.Discard;

namespace Sandbox
{
    public class HitCountPolicy : IDiscardPolicy<DataObject>
    {
        public int LookAhead { get; } = 1;

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
            return 10 * (listValue.HitCount / 10);
        }

        public int Change(object clusterData, DataObject listValue)
        {
            int count = (int) clusterData;
            DataObject dataObject = listValue;

            if (dataObject.HitCount > count + 10 && dataObject.HitCount < 100)
            {
                return 1;
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
