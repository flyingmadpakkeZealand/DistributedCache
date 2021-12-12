using System;
using System.Net;
using DistributedSetupLib.Configurer;

namespace TestMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            var masterController = SetupBuilder.GetBuilder.CreateMaster(7, new IPEndPoint(IPAddress.Loopback, 8), new IPEndPoint(IPAddress.Loopback, 9));
            masterController.Start();
            Console.WriteLine("Hello World!");
        }
    }
}
