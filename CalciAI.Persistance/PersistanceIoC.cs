using Microsoft.Extensions.DependencyInjection;
using CalciAI.Events;
using CalciAI.Models;
using CalciAI.Persistance.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CalciAI.Persistance
{
    public static class PersistanceIoC
    {
        public static void RegisterPersistance(this IServiceCollection services)
        {
            Console.WriteLine("Starting Persistance IoC.");

            var scanningAssemblies = new HashSet<Assembly>();

            foreach (string dllFile in Directory.GetFiles(CommonUtils.RootFolder, "*.dll"))
            {
                var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(dllFile);

                if (!CommonUtils.ASSEMBLIES.Contains(assembly.GetName().Name.Split(".")[0]))
                {
                    continue;
                }

                scanningAssemblies.Add(assembly);
            }

            foreach (var assembly in scanningAssemblies)
            {
                foreach (var type in assembly.ExportedTypes.Select(t => t.GetTypeInfo()).Where(t => t.IsClass && !t.IsAbstract))
                {
                    var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo()).ToArray();

                    if (interfaces.Length == 0)
                    {
                        continue;
                    }

                    foreach (var handlerType in interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMongoProcessor<>)))
                    {
                        services.AddTransient(handlerType.AsType(), type.AsType());
                    }

                    foreach (var handlerType in interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISqlProcessor<>)))
                    {
                        services.AddTransient(handlerType.AsType(), type.AsType());
                    }
                }
            }

            services.AddTransient<ISqlBus, SqlBus>();

            services.AddTransient<IBus, Bus>();

            Console.WriteLine("Scanning complete.");

           // var mongoConfig = FileConfigProvider.Load<MongoSetting>("mongo");

            var EncryptedSqlConnectionString = FileConfigProvider.Load<SqlSetting>("sql").ConnectionString;
            ConnStringStore.SqlConnectionString =  EncryptionDecryption.DecryptText(EncryptedSqlConnectionString);
            //ConnStringStore.SqlConnectionString = EncryptedSqlConnectionString; // EncryptionDecryption.DecryptText(EncryptedSqlConnectionString);

            Console.WriteLine("Initializing db connection...");

           // var mongoConnectionString = EncryptionDecryption.DecryptText(mongoConfig.ConnectionString);
            //var mongoDatabase = EncryptionDecryption.DecryptText(mongoConfig.Database);

           // services.AddSingleton(new MongoDB.Driver.MongoClient(mongoConnectionString).GetDatabase(mongoDatabase));
        }


        public static void RegisterMediator(this IServiceCollection services)
        {
            Console.WriteLine("Starting Register IoC.");

            var scanningAssemblies = new HashSet<Assembly>();

            foreach (string dllFile in Directory.GetFiles(CommonUtils.RootFolder, "*.dll"))
            {
                var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(dllFile);

                if (!CommonUtils.ASSEMBLIES.Contains(assembly.GetName().Name.Split(".")[0]))
                {
                    continue;
                }

                scanningAssemblies.Add(assembly);
            }

            foreach (var assembly in scanningAssemblies)
            {
                foreach (var type in assembly.ExportedTypes.Select(t => t.GetTypeInfo()).Where(t => t.IsClass && !t.IsAbstract))
                {
                    var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo()).ToArray();

                    if (interfaces.Length == 0)
                    {
                        continue;
                    }

                    foreach (var handlerType in interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotifyHandler<>)))
                    {
                        services.AddTransient(handlerType.AsType(), type.AsType());
                    }

                    if (interfaces.Contains(typeof(IService)))
                    {
                        services.AddTransient(interfaces.First(), type.AsType());
                    }
                    else if (interfaces.Contains(typeof(ISingletonService)))
                    {
                        services.AddSingleton(interfaces.First(), type.AsType());
                    }
                }
            }

            Console.WriteLine("Register IoC done.");
        }
    }
}