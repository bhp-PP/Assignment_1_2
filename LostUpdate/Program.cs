using System;
using System.Diagnostics;
using System.Threading;

namespace LostUpdate
{
    class Program
    {
        // shared field
        static int x = 0;

        static readonly object locker = new object();

        static SemaphoreSlim s = new SemaphoreSlim(1, 1);
        static Mutex m = new Mutex();

        static void Main()
        {
            Thread t1 = new Thread(() => IncrementX(1_000_000));
            Thread t2 = new Thread(() => IncrementX(1_000_000));

            Stopwatch sw = Stopwatch.StartNew();

            // start concurrent thread execution
            t1.Start();
            t2.Start();

            // let Main thread wait for t1 and t2 to finish.
            t1.Join();
            t2.Join();

            sw.Stop();

            // did we lose some updates?
            Console.WriteLine("X = {0:N0}", x);
            Console.WriteLine("Time = {0:F5} sec.", (sw.ElapsedMilliseconds / 1000f));
        }

        static void IncrementX(int numberOfTimes)
        {
            for (int i = 0; i < numberOfTimes; i++)
            {
                //x++;
                //IncrementWithMutex();
                IncrementWithSemaphoreSlim();
                //IncrementWithLock();
                //IncrementWithInterlocked();
            }
        }

        private static void IncrementWithInterlocked()
        {
            Interlocked.Increment(ref x);
        }

        private static void IncrementWithMutex()
        {
            m.WaitOne();
            x++;
            m.ReleaseMutex();
        }

        private static void IncrementWithSemaphoreSlim()
        {
            s.Wait();
            x++;
            s.Release();
        }

        private static void IncrementWithLock()
        {
            lock (locker)
            {
                x++;
            }
        }
    }
}

