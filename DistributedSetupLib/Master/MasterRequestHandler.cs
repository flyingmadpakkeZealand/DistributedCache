using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DistributedSetupLib.Connection;
using DistributedSetupLib.Misc;
using Newtonsoft.Json;

namespace DistributedSetupLib.Master
{
    public class MasterRequestHandler : IRequestHandler
    {
        public MaSlResponse HandleRequest(IDistributedNode context, string[] args)
        {
            args[0] = args[0].ToUpper();
            InsertNameHereCommand command = InsertNameHereCommandMethod.Convert(args[0]);

            if (context is MasterNode masterContext)
            {
                return command switch
                {
                    InsertNameHereCommand.Fetch => Fetch(masterContext, args),
                    InsertNameHereCommand.Set => Set(masterContext, args),
                    InsertNameHereCommand.Delete => Delete(masterContext, args),
                    InsertNameHereCommand.Cas => CompareAndSwap(masterContext, args),
                    _ => MaSlResponse.NotFoundResponse
                };
            }

            throw new ArgumentException("INVALID", nameof(context));
        }

        private MaSlResponse Fetch(MasterNode context, string[] args)
        {
            string fetch = args[0], key = args[1];

            //ImmutableList<IPEndPoint> slaveEndPoints = context.GetEndPoints;
            //int index = (int) context.GetSyncCount() % slaveEndPoints.Count;

            IPEndPoint slaveEndpoint = GetSlaveEndPoint(context, key);

            return ConnectWithNewClientAndGetResponse(slaveEndpoint, $"{fetch} {key}");
        }

        private MaSlResponse Set(MasterNode context, string[] args)
        {
            string set = args[0], key = args[1], value = args[2];

            //bool failed = false;
            //StringBuilder bodyBuilder = new StringBuilder();
            string request = $"{set} {key} {value}";
            //foreach (IPEndPoint slaveEndpoint in context.GetEndPoints)
            //{
            //    MaSlResponse response = ConnectWithNewClientAndGetResponse(slaveEndpoint, request);

            //    bodyBuilder.AppendLine($"{slaveEndpoint.Address} : {response.StatusCode.ToNumber()}");
            //    if (response.StatusCode == StatusCode.NotFound) failed = true;
            //}

            //return CombineMultiResponse(failed, bodyBuilder);
            IPEndPoint slaveEndPoint = GetSlaveEndPoint(context, key);

            return ConnectWithNewClientAndGetResponse(slaveEndPoint, request);
        }

        private MaSlResponse Delete(MasterNode context, string[] args)
        {
            string delete = args[0], key = args[1];

            //bool failed = false;
            //StringBuilder bodyBuilder = new StringBuilder();
            string request = $"{delete} {key}";
            //foreach (IPEndPoint slaveEndPoint in context.GetEndPoints)
            //{
            //    MaSlResponse response = ConnectWithNewClientAndGetResponse(slaveEndPoint, request);

            //    bodyBuilder.AppendLine($"{slaveEndPoint.Address} : {response.StatusCode.ToNumber()}");
            //    if (response.StatusCode == StatusCode.NotFound) failed = true;
            //}

            //return CombineMultiResponse(failed, bodyBuilder);

            IPEndPoint slaveEndPoint = GetSlaveEndPoint(context, key);

            return ConnectWithNewClientAndGetResponse(slaveEndPoint, request);
        }

        private MaSlResponse CompareAndSwap(MasterNode context, string[] args)
        {
            string cas = args[0], key = args[1], expectedValue = args[2], newValue = args[3];

            //bool failed = false;
            //StringBuilder bodyBuilder = new StringBuilder();
            string request = $"{cas} {key} {expectedValue} {newValue}";
            //foreach (IPEndPoint slaveEndPoint in context.GetEndPoints)
            //{
            //    MaSlResponse response = ConnectWithNewClientAndGetResponse(slaveEndPoint, request);

            //    bodyBuilder.AppendLine($"{slaveEndPoint.Address} : {response.StatusCode.ToNumber()}");
            //    if (response.StatusCode == StatusCode.NotFound) failed = true;
            //}

            //return CombineMultiResponse(failed, bodyBuilder);a

            IPEndPoint slaveEndPoint = GetSlaveEndPoint(context, key);

            return ConnectWithNewClientAndGetResponse(slaveEndPoint, request);
        }

        private MaSlResponse CombineMultiResponse(bool failed, StringBuilder bodyBuilder)
        {
            string body = bodyBuilder.ToString().TrimEnd(' ', '\n');
            MaSlResponse response = new MaSlResponse{Body = body};

            if (failed)
            {
                response.StatusCode = StatusCode.NotFound;
            }
            else if (!string.IsNullOrEmpty(body))
            {
                response.StatusCode = StatusCode.Ok;
            }
            else
            {
                response.StatusCode = StatusCode.Empty;
            }

            return response;
        }

        private MaSlResponse WriteRequestAndGetResponse(string request, StreamWriter sw, StreamReader sr)
        {
            sw.WriteLine(request);
            string reply = sr.ReadLine();
            return JsonConvert.DeserializeObject<MaSlResponse>(reply);
        }

        private string WriteRequestAndGetStringResponse(string request, StreamWriter sw, StreamReader sr)
        {
            sw.WriteLine(request);
            return sr.ReadLine();
        }

        private void ConnectWithNewClientAndAutoFlush(IPEndPoint ipEndPoint, out TcpClient client, out StreamWriter sw, out StreamReader sr)
        {
            client = new TcpClient();
            client.Connect(ipEndPoint);
            sw = new StreamWriter(client.GetStream()); //Note: Must be connected, otherwise exception.
            sw.AutoFlush = true;
            sr = new StreamReader(client.GetStream());
        } //TODO: Replace with connection factory.

        private MaSlResponse ConnectWithNewClientAndGetResponse(IPEndPoint ipEndPoint, string request)
        {
            using TcpClient client = new TcpClient();
            client.Connect(ipEndPoint);
            using StreamWriter sw = new StreamWriter(client.GetStream());
            sw.AutoFlush = true;
            using StreamReader sr = new StreamReader(client.GetStream());

            return WriteRequestAndGetResponse(request, sw, sr);
        }

        private int GetSection(string key, MasterNode context)
        {
            int hash = key.GetHashCode() % context.Sections;
            return Math.Abs(hash); //Note: hash must not be negative.
        }

        private IPEndPoint GetSlaveEndPoint(MasterNode context, string key)
        {
            ImmutableList<IPEndPoint> endPoints = context.EndPoints;

            return endPoints[GetSection(key, context) % endPoints.Count]; //Double modulus: Mostly just simulates a future implementation.
        }
    }
}
