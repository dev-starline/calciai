using CalciAI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.IO;

namespace CalciAI.Persistance
{
    public interface IBus
    {
        Task<ProcessResult> ExecuteMongo<T>(T command, OperatorUserId currentUser) where T : IMongoCommand;
    }

    public class Bus : IBus, IDisposable
    {
        private readonly ILoggerAdaptor<Bus> _logAdaptor;
        private readonly IServiceProvider _services;

        //private static readonly Counter RequestCountByMethod = Metrics.CreateCounter("command_total", "Number of commands received", new CounterConfiguration
        //{
        //    LabelNames = new[] { "Operator", "Command" }
        //});

        //private static readonly Histogram DurationHistogram = Metrics.CreateHistogram("command_duration", "Histogram of commands durations.", new HistogramConfiguration
        //{
        //    LabelNames = new[] { "Operator", "Command" }
        //});

        //private static readonly Counter FailedProcessCounter = Metrics.CreateCounter("command_failed_total", "Number of failed commands.", new CounterConfiguration
        //{
        //    LabelNames = new[] { "Operator", "Command" }
        //});

        public Bus(ILogger<Bus> logger, IServiceProvider services)
        {
            _logAdaptor = new LoggerAdaptor<Bus>(logger);
            _services = services;
        }

        public async Task<ProcessResult> ExecuteMongo<T>(T command, OperatorUserId currentUser) where T : IMongoCommand
        {
            //var operatorId = currentUser.OperatorId;
            //var commandName = command.GetType().ToString();
            //RequestCountByMethod.WithLabels(operatorId, commandName).Inc();

            //using (DurationHistogram.WithLabels(operatorId, commandName).NewTimer())
            //{
            try
            {
                var startTime = DateTime.UtcNow;

                var service = _services.GetService<IMongoProcessor<T>>();

                var validationResult = await service.Validate(command, currentUser);

                if (validationResult.IsSuccess)
                {
                    var resturnObj = await service.Execute(command, currentUser);
                    return resturnObj;
                }
                else
                {
                    return validationResult;
                }
            }
            catch (Exception ex)
            {
                //FailedProcessCounter.WithLabels(operatorId, commandName).Inc();

                _logAdaptor.LogError("{ex},{ERROR in execution. {currentUser} - {command}}", ex, currentUser, command);

                if (ex.GetType() == typeof(ApplicationException))
                {
                    throw;
                }

                return ProcessResult.Fail("Server", ex.Message);
            }
            //}
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
