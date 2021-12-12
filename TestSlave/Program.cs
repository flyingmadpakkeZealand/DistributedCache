using System;
using DistributedSetupLib.Configurer;

namespace TestSlave
{
    class Program
    {
        static void Main(string[] args)
        {
            var slaveController = SetupBuilder.GetBuilder.CreateSlave(8, 100);
            slaveController.Start();
            Console.WriteLine("Hello World!");
        }
    }
}
