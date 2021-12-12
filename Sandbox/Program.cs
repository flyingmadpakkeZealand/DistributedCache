using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CacheLib;
using DistributedSetupLib.Connection;
using Newtonsoft.Json;

namespace Sandbox
{
    class Program
    {
        public enum MyEnum
        {
            Ok = 200
        }

        private static int test = 0;

        static void Main(string[] args)
        {
            //TestClass t = new TestClass();
            //t.Start4();

            //SimpleCache<int, int> cache = new SimpleCache<int, int>(1);

            //bool result = cache.CompareAndSwap(42, default, 69);

            //result = cache.CompareAndSwap(43, default, 420);

            //TcpListener listener = TcpListener.Create(7);
            //listener.Start();
            //TcpClient client = listener.AcceptTcpClient();
            //StreamReader sr = new StreamReader(client.GetStream());

            //char c = (char) sr.Read();
            //Console.WriteLine(c);
            //uint n = uint.MaxValue;
            //Console.WriteLine(n+1);

            //List<int> twoNumbers = new List<int>(){0, 20};

            //Task<int>[] tasks = new Task<int>[20_000];
            //for (int i = 0; i < 20_000; i += 2)
            //{
            //    tasks[i] = Task.Run(() => twoNumbers[0] = 4);
            //    tasks[i + 1] = Task.Run(() => twoNumbers[1]);
            //}


            //Task.WaitAll(tasks);

            //Console.WriteLine(twoNumbers[0] + "" + twoNumbers[1]);

            //Console.WriteLine(MyEnum.Ok.ToString());

            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, 7);
            StreamWriter sw = new StreamWriter(client.GetStream());
            sw.AutoFlush = true;
            StreamReader sr = new StreamReader(client.GetStream());

            Person p = new Person
            {
                Name = "Obama",
                Age = 45,
                Atoms = long.MaxValue,
                Height = 1.89f,
                Weight = 84.45f
            };

            sw.WriteLine($"SET Person {JsonConvert.SerializeObject(p)}");

            while (true)
            {
                client = new TcpClient();
                client.Connect(IPAddress.Loopback, 7);
                sw = new StreamWriter(client.GetStream());
                sw.AutoFlush = true;
                sr = new StreamReader(client.GetStream());

                Console.WriteLine("Sandbox, Write command here:");
                string command = Console.ReadLine();

                sw.WriteLine(command);
                string response = sr.ReadLine();
                MaSlResponse maSlResponse = JsonConvert.DeserializeObject<MaSlResponse>(response);

                Console.WriteLine(maSlResponse);
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
