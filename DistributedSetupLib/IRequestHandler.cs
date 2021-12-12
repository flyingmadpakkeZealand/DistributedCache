using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedSetupLib.Connection;

namespace DistributedSetupLib
{
    public interface IRequestHandler
    {
        MaSlResponse HandleRequest(IDistributedNode context, string[] args);
    }
}
