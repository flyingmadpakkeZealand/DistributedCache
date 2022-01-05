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

        int Insertion(TValue value);

        int Deletion();

        object ClusterData(TValue value);

        int Change(object clusterData, TValue value);

        bool Allowance(object clusterData, TValue value);
    }
}
