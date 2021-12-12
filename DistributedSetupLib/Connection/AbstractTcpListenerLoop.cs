using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DistributedSetupLib.Connection
{
    public abstract class AbstractTcpListenerLoop<T> : IDistributedNode
    {
        private readonly int _port;

        protected AbstractTcpListenerLoop(int port)
        {
            _port = port;
        }

        public void Start()
        {
            TcpListener listener = TcpListener.Create(_port);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                Task.Run(() =>
                {
                    using (client) HandleClient(client);
                });
            }
        }

        private void HandleClient(TcpClient client)
        {
            using StreamReader sr = new StreamReader(client.GetStream());
            using StreamWriter sw = new StreamWriter(client.GetStream());
            bool keepAlive = true;

            while (keepAlive)
            {
                string command = sr.ReadLine();
                T response = InterpretCommand(command, out keepAlive);
                sw.WriteLine(response);
                sw.Flush();
            }
        }

        protected abstract T InterpretCommand(string command, out bool keepAlive);
    }
}
