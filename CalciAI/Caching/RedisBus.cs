using StackExchange.Redis;
using System;
using System.Diagnostics;

namespace CalciAI.Caching
{
    public static class RedisBus
    {
        private static readonly Lazy<ConnectionMultiplexer> lazyConnection = CreateConnection();

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        private static Lazy<ConnectionMultiplexer> CreateConnection()
        {
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                var config = FileConfigProvider.Load<RedisSetting>("redis.bus");

                var configurationOptions = new ConfigurationOptions
                {
                    EndPoints = { EncryptionDecryption.DecryptText(config.EndPoints) },
                    Password = EncryptionDecryption.DecryptText(config.Password),
                    DefaultDatabase = config.DefaultDatabase,
                    AbortOnConnectFail = config.AbortOnConnectFail
                };

                ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);

                Console.WriteLine("Connection to Redis Bus");

                var watch = new Stopwatch();

                watch.Start();

                var connection = ConnectionMultiplexer.Connect(configurationOptions);

                watch.Stop();

                Console.WriteLine($"Redis Bus connected in : {watch.ElapsedMilliseconds}ms");

                return connection;
            });
        }

        public static IDatabase RedisCache => Connection.GetDatabase();

        public static ISubscriber Subscriber => Connection.GetSubscriber();
    }

}
