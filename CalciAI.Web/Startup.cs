using CalciAI.Web.AuthMiddleware;
using CalciAI.Web.LoggingMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalciAI.Serialization;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Text.Json.Serialization;

namespace CalciAI.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                    .AddXmlSerializerFormatters()
                    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()))
                    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter()))
                    //.AddJsonOptions(options => options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals)
                    .AddXmlDataContractSerializerFormatters();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

           // bool scanDisabledOperator = Configuration.GetSection("ScanDisabledOperator").Exists() && (bool)Configuration.GetValue(typeof(bool), "ScanDisabledOperator");
          //  OperatorProvider.Refresh(scanDisabledOperator).Wait();

            services.AddCors(o => o.AddPolicy(Auth.AuthFields.CORS_POLICY, builder =>
            {
                Console.WriteLine($"Loading Operator Cache.");

                var corsUrls = OperatorProvider.GetUrlsForCors();

                Console.WriteLine($"Adding CORS for domains: {Environment.NewLine}{string.Join(Environment.NewLine, corsUrls)}");

                //For debug purpose
                corsUrls = (corsUrls.Append("http://localhost:3000")
                                    .Append("http://localhost:3001")
                                    .Append("http://localhost:3002")
                                    .Append("http://localhost:3003")
                                    .Append("http://localhost:3004")
                                    .Append("http://localhost:3005")
                                    ).ToArray();

                builder.WithOrigins(corsUrls)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(60 * 60))
                    .AllowAnyMethod()
                    .AllowAnyHeader();

            }));

            //services.Configure<GzipCompressionProviderOptions>(options =>
            //{
            //    options.Level = CompressionLevel.Fastest;
            //});
            //services.AddResponseCompression(options =>
            //{
            //    options.Providers.Add<GzipCompressionProvider>();
            //    options.EnableForHttps = true;
            //});

            //var loadBalancerSetting = new LoadBalancerSetting();
            //Configuration.GetSection(LoadBalancerSetting.SectionName).Bind(loadBalancerSetting);

            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    options.ForwardLimit = 2;

            //    options.RequireHeaderSymmetry = false;

            //    options.ForwardedHeaders = ForwardedHeaders.All;

            //    options.AllowedHosts = new List<string>();

            //    if (!string.IsNullOrEmpty(loadBalancerSetting.Cidr))
            //    {
            //        Console.WriteLine($"Adding Known Network: {loadBalancerSetting.Cidr}");

            //        var networkData = loadBalancerSetting.Cidr.Split("/");
            //        var address = IPAddress.Parse(networkData[0]);
            //        var mask = int.Parse(networkData[1]);
            //        var network = new IPNetwork(address, mask);
            //        options.KnownNetworks.Add(network);
            //    }

            //    if (loadBalancerSetting.Addresses != null)
            //    {
            //        foreach (var address in loadBalancerSetting.Addresses)
            //        {
            //            Console.WriteLine($"Adding Known Proxies: {address}");
            //            options.KnownProxies.Add(IPAddress.Parse(address));
            //        }
            //    }
            //});

            var authTypes = new List<string>();
            Configuration.GetSection("AuthTypes").Bind(authTypes);

            if (authTypes.Count > 0)
            {
                // added S2S Auth
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CustomAuthOptions.DefaultScheme;
                    options.DefaultChallengeScheme = CustomAuthOptions.DefaultScheme;
                })
                // Call custom authentication extension method
                .AddCustomAuth(options =>
                {
                    // Configure single or multiple passwords for authentication
                    options.AuthTypes = authTypes.ToArray();
                    options.IsHostOrigin = Configuration.GetValue<bool>("IsHostOrigin");
                });
            }

            services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
                .AddApplicationPart(Assembly.GetEntryAssembly())
                .AddControllersAsServices();

            services.AddMemoryCache();

            services.AddHealthChecks()
                .AddCheck<ApiHealthCheck>("api")
                .ForwardToPrometheus();

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.Zero;
            });

            //Adding servies
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.RegisterMediator();

            var iocChildren = Configuration.GetSection("IoC").GetChildren().ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in iocChildren)
            {
                var iface = Type.GetType(item.Key, true);
                var impl = Type.GetType(item.Value, true);

                services.AddTransient(iface, impl);
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILogger<Startup> logger, ILoggerFactory logFactory, IWebHostEnvironment env)
        {
            var serviceProvider = app.ApplicationServices;

            ApplicationLogging.SetLoggerFactory(logFactory);

            //logger.LogInformation("Environment {EnvironmentName}", env.EnvironmentName);

            app.UseForwardedHeaders();

            app.UseExceptionHandler(AppStartupUtils.ConfigureWebException);

            app.UseMiddleware<ApiMonitorMiddleware>();

            app.UseCors(Auth.AuthFields.CORS_POLICY);

            app.UseRequestResponseLogging();

            app.UseRouting();
            var authTypes = new List<string>();
            Configuration.GetSection("AuthTypes").Bind(authTypes);

            if (authTypes.Count > 0)
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/");
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });

            //app.UseResponseCompression();

            AppStartupUtils.PrintInfo(serviceProvider, Configuration);
        }

        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    var serviceProvider = app.ApplicationServices;

        //    app.UseForwardedHeaders();

        //    app.UseExceptionHandler(AppStartupUtils.ConfigureWebException);

        //    app.UseMiddleware<ApiMonitorMiddleware>();

        //    app.UseCors(Auth.AuthFields.CORS_POLICY);

        //    app.UseRequestResponseLogging();

        //    app.UseRouting();
        //    var authTypes = new List<string>();
        //    Configuration.GetSection("AuthTypes").Bind(authTypes);

        //    if (authTypes.Count > 0)
        //    {
        //        app.UseAuthentication();
        //        app.UseAuthorization();
        //    }

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapHealthChecks("/");
        //        endpoints.MapControllers();
        //        endpoints.MapMetrics();
        //    });

        //    AppStartupUtils.PrintInfo(serviceProvider, Configuration);
        //}
    }
}
