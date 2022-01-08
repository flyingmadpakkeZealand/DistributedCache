using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib.Discard
{
    public interface IDiscardPolicy<in TValue>
    {
        Dimension ThisDimension { get; }

        int LookAhead { get; }

        ClusterPosition Insertion(TValue value);

        ClusterPosition Deletion();

        object ClusterData(TValue value);

        Dimension ChangeTo(object clusterData, TValue value);

        bool Allowance(object clusterData, TValue value);
    }
}
