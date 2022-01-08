using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox
{
    class TestClass
    {
        private ConcurrentDictionary<int, int> dictionary = new ConcurrentDictionary<int, int>();

        private int normalInt;

        private bool done;

        public void Nothing<T>(T someType)
        {
            T newType = someType;
            newType.ToString();
        }

        public void Start()
        {
            new Task(AddToDictionary).Start();
            new Task(ReadCount).Start();
        }

        private void ReadCount()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(dictionary.Count);
                Thread.Sleep(500);
            }
        }

        private void AddToDictionary()
        {
            for (int i = 0; i < 100_000_000; i++)
            {
                bool added = dictionary.TryAdd(i, i);
                if (!added)
                {
                    Console.WriteLine("NOT ADDED");
                }
            }
        }

        public void Start2()
        {
            new Task(IncInt).Start();
            new Task(ReadInt).Start();
        }

        public void Start3()
        {
            new Task(PrintWhenDone).Start();
            new Task(IncInt).Start();
        }

        public void Start4()
        {
            new Thread(IncInt).Start();
            new Thread(PrintWhenDone).Start();
        }

        private void ReadInt()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(normalInt);
                Thread.Sleep(500);
            }
        }

        private void PrintWhenDone()
        {
            while (!done)
            {
                
            }

            Console.WriteLine("Done!");
        }

        private void IncInt()
        {
            for (int i = 0; i < 1_000_000_000; i++)
            {
                normalInt++;
            }

            Console.WriteLine(normalInt);
            done = true;
        }
    }
}
