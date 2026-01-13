using System;
using System.Threading;

namespace CalciAI
{
    public static class ThreadPoolManager
    {
        public static void IncreaseMinWorker(int times)
        {
            // Get the current settings.
            ThreadPool.GetMinThreads(out int minWorker, out int minIOC);
            // Change the minimum number of worker threads to four, but
            // keep the old setting for minimum asynchronous I/O 
            // completion threads.

            var newMinWorker = minWorker * times;

            if (ThreadPool.SetMinThreads(newMinWorker, minIOC))
            {
                Console.WriteLine($"Min worker set to {newMinWorker}, {minIOC}");
            }
            else
            {
                Console.WriteLine($"Error in setting min worker threads. Current: {newMinWorker}, {minIOC}");
            }
        }
    }
}
