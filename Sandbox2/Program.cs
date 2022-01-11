using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Sandbox2
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcurrentDictionary<string, string> concurrentDictionary = 
                new ConcurrentDictionary<string, string>();

            while (true)
            {
                string oldValue = concurrentDictionary["Animal"];

                if (oldValue is null)
                {
                    if (concurrentDictionary.TryAdd("Animal", "Croc"))
                    {
                        //return null;
                    }

                    continue;
                }
            
                if (concurrentDictionary.TryUpdate("Animal", "croc", oldValue))
                {
                    //return oldValue;
                }
            }

            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 7);
            //TcpClient client = new TcpClient();

            //client.Connect(endPoint);
            //StreamWriter sw = new StreamWriter(client.GetStream());
            //sw.Write("Hi\nWhat is your name?\nI like you\0");
            //sw.Flush();

            //Console.ReadLine();
        }
    }
}
