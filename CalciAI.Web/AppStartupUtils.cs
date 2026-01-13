using CalciAI.Caching;
using CalciAI.Events;
using CalciAI.Helpers;
using CalciAI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace CalciAI.Web
{
    public static class AppStartupUtils
    {
        public static void InitializeCachingConnection()
        {
            if(FileConfigProvider.Exists("redis"))
            {
                RedisStore.RedisCache.Execute("PING");
            }
            
            //if(FileConfigProvider.Exists("redis.bus"))
            //{
            //    RedisBus.RedisCache.Execute("PING");
            //}
        }

        public static void ConfigureWorkerConfiguration(HostBuilderContext hostBuilderContext, IConfigurationBuilder configureBuilder)
        {
            InitializeCachingConnection();

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                Console.WriteLine("Shutting down...");
            };

            var env = hostBuilderContext.HostingEnvironment;
            var currentDirectory = Directory.GetCurrentDirectory();
            var codeBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(CommonUtils.Banner(env.ContentRootPath));
            Console.WriteLine($"EnvironmentName: {env.EnvironmentName}");
            Console.WriteLine($"EnvironmentVariable: {CommonUtils.EnvironmentName}");
            Console.WriteLine($"Root: {env.ContentRootPath}");
            Console.WriteLine($"Current Directory: {currentDirectory}");
            Console.WriteLine($"Codebase: {codeBase}");

            if (env.EnvironmentName != CommonUtils.EnvironmentName)
            {
                throw new TargetException($"Environment does not match. Default: {env.EnvironmentName}, System: {CommonUtils.EnvironmentName}");
            }

            Directory.SetCurrentDirectory(codeBase);
            configureBuilder.SetBasePath(codeBase);
            configureBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configureBuilder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
            configureBuilder.AddEnvironmentVariables();
        }

        public static void ConfigureWebConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder)
        {
            InitializeCachingConnection();

            Console.WriteLine($"Starting Web configuration.");

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                Console.WriteLine("Shutting down...");
            };

            var env = context.HostingEnvironment;
            var currentDirectory = Directory.GetCurrentDirectory();
            var codeBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(CommonUtils.Banner(env.ContentRootPath));
            Console.WriteLine($"EnvironmentName: {env.EnvironmentName}");
            Console.WriteLine($"EnvironmentVariable: {CommonUtils.EnvironmentName}");
            Console.WriteLine($"Root: {env.ContentRootPath}");
            Console.WriteLine($"Current Directory: {currentDirectory}");
            Console.WriteLine($"Codebase: {codeBase}");

            if (env.EnvironmentName != CommonUtils.EnvironmentName)
            {
                throw new TargetException($"Environment does not match. Default: {env.EnvironmentName}, System: {CommonUtils.EnvironmentName}");
            }

            Directory.SetCurrentDirectory(codeBase);
            builder.SetBasePath(codeBase);
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
            builder.AddEnvironmentVariables();

            Console.WriteLine($"Web configuration done.");
        }

        public static void ConfigureWebException(IApplicationBuilder builder)
        {
            Console.WriteLine("Starting global Exception configuration.");

            var logger = builder.ApplicationServices.GetService<ILogger<ApiControllerBase>>();

            builder.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature == null)
                {
                    return;
                }

                var request = context.Request;

                var path = request.Path;
                var error = contextFeature.Error;
                var code = context.Response.StatusCode;
                var type = error.GetType().ToString();
                var message = error.Message;

                ApplicationLogging.CreateLogger("ExceptionHandler").LogError("{path},{code},{type}-{message}", path, code, type, message);

                var requestIdFeature = context.Features.Get<IHttpRequestIdentifierFeature>();

                if (requestIdFeature?.TraceIdentifier != null)
                {
                    var status = context.Response.StatusCode;

                    string payload = null;

                    if (context.Request.Body.CanSeek)
                    {
                        context.Request.Body.Seek(0, SeekOrigin.Begin);
                        using var streamReader = new StreamReader(context.Request.Body, Encoding.UTF8);
                        payload = streamReader.ReadToEndAsync().Result;
                    }

                    var data = new
                    {
                        requestIdFeature.TraceIdentifier,
                        AppDomain.CurrentDomain.FriendlyName,
                        request.Method,
                        path = request.Path.Value,
                        qs = request.QueryString.Value,
                        request.ContentType,
                        payload,
                        status,
                        type,
                        message
                    };

                    logger.LogError("Payload: {data}", JsonSerializer.Serialize(data));
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var responseObj = ProcessResult.Fail("Server", contextFeature.Error.Message);
                await context.Response.WriteAsync(JsonSerializer.Serialize(responseObj));
            });

            Console.WriteLine("Global Exception configuration done.");
        }

        /// <summary>
        /// http://codebuckets.com/2020/05/29/dynamically-loading-assemblies-for-dependency-injection-in-net-core/
        /// </summary>
        /// <param name="services"></param>
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

        public static void PrintInfo(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var endPoint = configuration.GetValue<string>("Kestrel:EndPoints:Http:Url");
            Console.WriteLine($"WEB SERVER RUNNING ON:{endPoint}");

            var routeService = serviceProvider.GetService<IActionDescriptorCollectionProvider>();

            var actionDescriptors = routeService.ActionDescriptors;

            var routes = new List<dynamic[]>
            {
                new dynamic[] { "Controller", "Method", "CORS", "Template" }
            };

            foreach (var descriptors in actionDescriptors.Items)
            {
                if (descriptors.EndpointMetadata.FirstOrDefault(x => x.GetType().Equals(typeof(HttpMethodMetadata))) is not HttpMethodMetadata metaData)
                {
                    continue;
                }

                var cors = metaData.AcceptCorsPreflight;
                var controller = descriptors.RouteValues["Controller"];
                var method = descriptors.RouteValues["Action"];
                var template = $"{string.Join(',', metaData.HttpMethods)}-{descriptors.AttributeRouteInfo?.Template}";
                routes.Add(new dynamic[] { controller, method, cors, template });
            }

            //Printing Routes:
            Console.WriteLine(ArrayPrinter.GetDataInTableFormat(routes));
        }
    }
}
