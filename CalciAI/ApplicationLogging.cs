using Microsoft.Extensions.Logging;

namespace CalciAI
{
    public static class ApplicationLogging
    {
        private static ILoggerFactory AppLoggerFactory { get; set; } = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            // Clear Microsoft's default providers (like eventlogs and others)
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            }).SetMinimumLevel(LogLevel.Trace);
        });

        public static void SetLoggerFactory(ILoggerFactory factory)
        {
            AppLoggerFactory = factory;
        }

        public static ILogger<T> CreateLogger<T>() => AppLoggerFactory.CreateLogger<T>();

        public static ILogger CreateLogger(string loggerName) => AppLoggerFactory.CreateLogger(loggerName);
    }
}
