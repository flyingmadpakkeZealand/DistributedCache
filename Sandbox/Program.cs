using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using CacheLib;
using CacheLib.Discard;
using CacheLib.Expiry;
using DistributedSetupLib.Connection;
using Newtonsoft.Json;

namespace Sandbox
{
    class Program
    {
        public abstract class MyClass
        {
            
        }

        public class MyClass2 : MyClass
        {
            
        }

        public enum MyEnum
        {
            Ok = 200
        }

        private static int test = 0;

        static void Main(string[] args)
        {
            SimpleCache<string, int> cache = new SimpleCache<string, int>(100);

            //RecursiveLinkedListWorker worker = new RecursiveLinkedListWorker();
            //var result1 = worker.Lru();
            //var result2 = worker.Lfu();
            //var result3 = worker.LfRu();

            //Func<string, Person> personCreator = str => new Person(str);

            //DiscardGraph<MyClass> graph = new DiscardGraph<MyClass>();
            //graph.AddNew(new MyClass2());

            //TestClass t = new TestClass();
            //t.Start4();

            //SimpleCache<int, int> cache = new SimpleCache<int, int>(1);

            Dimension d = new Dimension();

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

            //DataObject do1 = new DataObject();
            //DataObject do2 = new DataObject();
            //RecursiveLinkedList recursiveLinkedList = new RecursiveLinkedList(new HitCountPolicy(), new MockPolicy());
            //var result = recursiveLinkedList.AddNew(do1);
            //var result2 = recursiveLinkedList.AddNew(do2);
            
            //do1.OnSomeCommandInvoked(15);
            //recursiveLinkedList.UpdateNode(ref result);
            //recursiveLinkedList.UpdateNode(ref result2);

            //var result3 = recursiveLinkedList.RemoveEntries(2);

            //DataObject do3 = new DataObject();
            //recursiveLinkedList.AddNew(do3);
            

            //Task.WaitAll(tasks);

            //Console.WriteLine(twoNumbers[0] + "" + twoNumbers[1]);

            //Console.WriteLine(MyEnum.Ok.ToString());

            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, 7);
            StreamWriter sw = new StreamWriter(client.GetStream());
            sw.AutoFlush = true;
            StreamReader sr = new StreamReader(client.GetStream());

            Animal animal = new Animal
            {
                Genus = "Crocodylus",
                Species = "Johnstoni",
                Gender = "Female",
                Length = 1.6,
                Weight = 35
            };

            sw.WriteLine($"SET Animal {JsonConvert.SerializeObject(animal)}");

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
