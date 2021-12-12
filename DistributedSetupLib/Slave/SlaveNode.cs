using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLib;
using DistributedSetupLib.Connection;
using Newtonsoft.Json;

namespace DistributedSetupLib.Slave
{
    public class SlaveNode : AbstractTcpListenerLoop<string>
    {
        private readonly IRequestHandler _slaveRequestHandler;

        public ICache<string, string> Cache { get; }

        public SlaveNode(int port, IRequestHandler slaveRequestHandler, ICache<string, string> cache) : base(port)
        {
            _slaveRequestHandler = slaveRequestHandler;
            Cache = cache;
        }

        protected override string InterpretCommand(string command, out bool keepAlive)
        {
            MaSlResponse response = 
                _slaveRequestHandler.HandleRequest(this, 
                    command.Split(' ', StringSplitOptions.RemoveEmptyEntries));

            keepAlive = false;
            return JsonConvert.SerializeObject(response);
        }
    }
}
