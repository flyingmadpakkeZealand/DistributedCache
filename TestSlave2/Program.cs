using System;
using DistributedSetupLib.Configurer;

namespace TestSlave2
{
    class Program
    {
        static void Main(string[] args)
        {
            var slaveNode = SetupBuilder.GetBuilder.CreateSlave(9, 100);
            slaveNode.Start();
        }
    }
}
