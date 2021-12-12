using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CacheLib;
using DistributedSetupLib.Master;
using DistributedSetupLib.Slave;

namespace DistributedSetupLib.Configurer
{
    public class SetupBuilder
    {
        public static SetupBuilder GetBuilder { get; } = new SetupBuilder();

        private MasterRequestHandler _masterRequestHandler;
        private SlaveRequestHandler _slaveRequestHandler;

        private SetupBuilder(){}

        public IDistributedNode CreateMaster(int ownPort, params IPEndPoint[] slaves)
        {
            MasterNode masterNode = new MasterNode(ownPort, 
                LazyInit(ref _masterRequestHandler, new MasterRequestHandler()), 
                slaves);

            return masterNode;
        }

        public IDistributedNode CreateSlave(int ownPort, int cacheSize)
        {
            SlaveNode slaveNode = new SlaveNode(ownPort,
                LazyInit(ref _slaveRequestHandler, new SlaveRequestHandler()),
                new SimpleCache<string, string>(cacheSize));

            return slaveNode;
        }

        private T LazyInit<T>(ref T variable, T obj)
        {
            variable ??= obj;
            return variable;
        }
    }
}