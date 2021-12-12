using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistributedSetupLib.Connection;
using DistributedSetupLib.Misc;
using Newtonsoft.Json;

namespace DistributedSetupLib.Master
{
    public class MasterNode : AbstractTcpListenerLoop<string>
    {
        private readonly IRequestHandler _masterRequestHandler;

        private uint _syncCounter; //Note: Only interlocked access!
        private volatile ImmutableList<IPEndPoint> _endPoints; //Note: How does volatile actually work when the getter method is a new method frame each time?

        public ImmutableList<IPEndPoint> GetEndPoints => _endPoints;

        public MasterNode(int port, IRequestHandler masterRequestHandler, IEnumerable<IPEndPoint> slaveEndPoints) :
            base(port)
        {
            _masterRequestHandler = masterRequestHandler;

            if (slaveEndPoints is ImmutableList<IPEndPoint> cast)
            {
                _endPoints = cast;
            }
            else
            {
                _endPoints = ImmutableList.Create(slaveEndPoints.ToArray());
            }

            _syncCounter = uint.MaxValue;
        }

        public MasterNode(int port, IRequestHandler masterRequestHandler) : this(port, masterRequestHandler, new ConcurrentStack<IPEndPoint>()) { }

        public uint GetSyncCount()
        {
            return Interlocked.Increment(ref _syncCounter);
        }

        protected override string InterpretCommand(string command, out bool keepAlive)
        {
            MaSlResponse maSlResponse =
                _masterRequestHandler.HandleRequest(this, 
                    command.Split(' ', StringSplitOptions.RemoveEmptyEntries));


            keepAlive = false;
            return JsonConvert.SerializeObject(maSlResponse); //TODO: Potentially minimize serialization.
        }
    }
}
