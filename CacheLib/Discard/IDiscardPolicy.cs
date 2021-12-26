using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib.Discard
{
    public interface IDiscardPolicy<in TValue>
    {
        int LookAhead { get; }

        int Insertion(TValue listValue);

        int Deletion();

        object ClusterData(TValue listValue);

        int Change(object clusterData, TValue listValue);

        bool Allowance(object clusterData, TValue listValue);
    }
}
