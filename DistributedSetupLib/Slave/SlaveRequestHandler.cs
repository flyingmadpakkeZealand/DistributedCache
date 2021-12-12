using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedSetupLib.Connection;
using DistributedSetupLib.Master;
using DistributedSetupLib.Misc;

namespace DistributedSetupLib.Slave
{
    public class SlaveRequestHandler : IRequestHandler
    {
        public MaSlResponse HandleRequest(IDistributedNode context, string[] args)
        {
            InsertNameHereCommand command = InsertNameHereCommandMethod.Convert(args[0]);
            if (args.Length > 2 && args[2] == "%null") args[2] = null;

            if (context is SlaveNode slaveContext)
            {
                return command switch
                {
                    InsertNameHereCommand.Fetch => Fetch(slaveContext, args),
                    InsertNameHereCommand.Set => Set(slaveContext, args),
                    InsertNameHereCommand.Delete => Delete(slaveContext, args),
                    InsertNameHereCommand.Cas => CompareAndSwap(slaveContext, args),
                    _ => MaSlResponse.NotFoundResponse
                };
            }

            throw new ArgumentException("INVALID", nameof(slaveContext));
        }

        private MaSlResponse Fetch(SlaveNode context, string[] args)
        {
            string key = args[1];

            bool entryExist = context.Cache.Fetch(key, out string entry);
            MaSlResponse response = new MaSlResponse
            {
                StatusCode = entryExist ? StatusCode.Ok : StatusCode.NotFound,
                Body = entry
            };

            return response;
        }

        private MaSlResponse Set(SlaveNode context, string[] args)
        {
            string key = args[1], value = args[2];

            MaSlResponse response = MaSlResponse.EmptyResponse;
            bool added = context.Cache.Set(key, value);

            if (!added) response.StatusCode = StatusCode.NotFound;
            return response;
        }

        private MaSlResponse Delete(SlaveNode context, string[] args)
        {
            string key = args[1];

            bool added = context.Cache.Delete(key);

            return new MaSlResponse
            {
                StatusCode = added ? StatusCode.Empty : StatusCode.NotFound
            };
        }

        private MaSlResponse CompareAndSwap(SlaveNode context, string[] args)
        {
            string key = args[1], expectedValue = args[2], newValue = args[3];

            bool swapped = context.Cache.CompareAndSwap(key, expectedValue, newValue);

            return new MaSlResponse
            {
                StatusCode = swapped ? StatusCode.Empty : StatusCode.NotFound
            };
        }
    }
}
