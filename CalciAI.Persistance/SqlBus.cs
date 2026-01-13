using CalciAI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Threading.Tasks;

namespace CalciAI.Persistance
{
    public interface ISqlBus
    {
        Task<ProcessResult> ExecuteSql<T>(T command, string operatorId) where T : ISqlCommand;
    }

    public class SqlBus : ISqlBus, IDisposable
    {
        private readonly ILoggerAdaptor<SqlBus> _logAdaptor;
        private readonly IServiceProvider _services;

        private static readonly Counter RequestCountByMethod = Metrics.CreateCounter("command_total", "Number of commands received", new CounterConfiguration
        {
            LabelNames = new[] { "Operator", "Command" }
        });

        private static readonly Histogram DurationHistogram = Metrics.CreateHistogram("command_duration", "Histogram of commands durations.", new HistogramConfiguration
        {
            LabelNames = new[] { "Operator", "Command" }
        });

        private static readonly Counter FailedProcessCounter = Metrics.CreateCounter("command_failed_total", "Number of failed commands.", new CounterConfiguration
        {
            LabelNames = new[] { "Operator", "Command" }
        });

        public SqlBus(ILogger<SqlBus> logger, IServiceProvider services)
        {
            _logAdaptor = new LoggerAdaptor<SqlBus>(logger);
            _services = services;
        }

        public async Task<ProcessResult> ExecuteSql<T>(T command, string operatorId) where T : ISqlCommand
        {
            var commandName = command.GetType().ToString();
            RequestCountByMethod.WithLabels("", commandName).Inc();

            using (DurationHistogram.WithLabels("", commandName).NewTimer())
            {
                try
                {
                    var service = _services.GetService<ISqlProcessor<T>>();

                    var validationResult = await service.Validate(command);

                    if (validationResult.IsSuccess)
                    {
                        return await service.Execute(command);
                    }
                    else
                    {
                        return validationResult;
                    }
                }
                catch (Exception ex)
                {
                    FailedProcessCounter.WithLabels("", commandName).Inc();

                    _logAdaptor.LogError("{ex},{ERROR in execution. - {command}}",ex, command);

                    if (ex.GetType() == typeof(ApplicationException))
                    {
                        throw;
                    }

                    return ProcessResult.Fail("Server", ex.Message);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
