using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CalciAI.Caching
{
    public static class RedLock
    {
        public static Task<IDisposable> AcquireLockAsync(this IDatabaseAsync db, string key, TimeSpan? expiry = null, TimeSpan? retryTimeout = null)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return DataCacheLock.AcquireAsync(db, key, expiry, retryTimeout);

        }

        private sealed class DataCacheLock : IDisposable
        {
            private readonly IDatabaseAsync _db;
            public readonly RedisKey Key;
            public readonly RedisValue Value;
            public readonly TimeSpan? Expiry;

            private DataCacheLock(IDatabaseAsync db, string key, TimeSpan? expiry)
            {
                _db = db;
                Key = "ninlock:" + key;
                Value = Guid.NewGuid().ToString();
                Expiry = expiry;
            }

            public static async Task<IDisposable> AcquireAsync(IDatabaseAsync db, string key, TimeSpan? expiry, TimeSpan? retryTimeout)
            {
                DataCacheLock dataCacheLock = new(db, key, expiry);

                Debug.WriteLine(dataCacheLock.Key.ToString() + ":" + dataCacheLock.Value.ToString());

                async Task<bool> task()
                {
                    try
                    {
                        return await db.LockTakeAsync(dataCacheLock.Key, dataCacheLock.Value, dataCacheLock.Expiry ?? TimeSpan.MaxValue);
                    }
                    catch
                    {
                        return false;
                    }
                }

                await RetryUntilTrueAsync(task, retryTimeout, _random);
                return dataCacheLock;
            }

            public void Dispose()
            {
                Debug.WriteLine("Disposing - release the lock:" + Value);
                _db.LockReleaseAsync(Key, Value).Wait();
            }
        }

        private static readonly Random _random = new();

        private static async Task<bool> RetryUntilTrueAsync(Func<Task<bool>> task, TimeSpan? retryTimeout, Random _random)
        {
            int i = 0;
            DateTime utcNow = DateTime.UtcNow;
            while (!retryTimeout.HasValue || DateTime.UtcNow - utcNow < retryTimeout.Value)
            {
                i++;
                if (await task())
                {
                    return true;
                }
                var waitFor = _random.Next((int)Math.Pow(i, 2), (int)Math.Pow(i + 1, 2) + 1);
                Debug.WriteLine(waitFor);
                await Task.Delay(waitFor);
            }

            throw new TimeoutException(string.Format("Exceeded timeout of {0}", retryTimeout.Value));
        }
    }
}
