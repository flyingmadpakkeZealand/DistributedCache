using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Sandbox2
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 7);
            TcpClient client = new TcpClient();

            client.Connect(endPoint);
            StreamWriter sw = new StreamWriter(client.GetStream());
            sw.Write("Hi\nWhat is your name?\nI like you\0");
            sw.Flush();

            Console.ReadLine();
        }
    }
}
