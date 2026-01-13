using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CalciAI.Web
{
    /// <summary>
    /// var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
    /// string controllerName = controllerActionDescriptor?.ControllerName;
    /// string actionName = controllerActionDescriptor?.ActionName;
    /// </summary>
    public class ApiMonitorMiddleware
    {
        // Name of the Response Header, Custom Headers starts with "X-"  
        private const string RESPONSE_HEADER_RESPONSE_TIME = "X-Response-Time-ms";
        private const string CONTENT_HEADER_NAME = "Content-Type";

        // Handle to the next Middleware in the pipeline  
        private readonly RequestDelegate _next;
        private readonly ILoggerAdaptor<ApiMonitorMiddleware> _logAdaptor;

        private static readonly Counter RequestCountByMethod = Metrics.CreateCounter("requests_total", "Number of requests received", new CounterConfiguration
        {
            LabelNames = new[] { "AppName" }
        });

        private static readonly Histogram DurationHistogram = Metrics.CreateHistogram("requests_duration", "Histogram of processing durations.", new HistogramConfiguration
        {
            LabelNames = new[] { "AppName" }
        });

        private static readonly Counter FailedProcessCounter = Metrics.CreateCounter("requests_failed_total", "Number of failed operations.", new CounterConfiguration
        {
            LabelNames = new[] { "AppName" }
        });

        public ApiMonitorMiddleware(RequestDelegate next, ILogger<ApiMonitorMiddleware> logger)
        {
            _next = next;
            _logAdaptor = new LoggerAdaptor<ApiMonitorMiddleware>(logger);
        }

        public Task InvokeAsync(HttpContext context)
        {
            // Start the Timer using Stopwatch  
            var watch = new Stopwatch();
            watch.Start();

            var request = context.Request;
            var rcType = request.ContentType;
            var rcHeader = context.Request.Headers[CONTENT_HEADER_NAME];

            if (rcType == "application/json" || rcHeader == "application/json")
            {
                //NOTE: changed error to info
                _logAdaptor.LogTrace("{requestPath} - missing content formatter:utf-8", request.Path.Value);
                request.ContentType = "application/json; charset=utf-8";
            }

            context.Response.OnStarting(() =>
            {
                // Stop the timer information and calculate the time   
                watch.Stop();
                var responseTimeForCompleteRequest = watch.ElapsedMilliseconds;
                // Add the Response time information in the Response headers.   
                context.Response.Headers[RESPONSE_HEADER_RESPONSE_TIME] = responseTimeForCompleteRequest.ToString();

                RequestCountByMethod.WithLabels(AppDomain.CurrentDomain.FriendlyName).Inc();

                DurationHistogram.WithLabels(AppDomain.CurrentDomain.FriendlyName).Observe(responseTimeForCompleteRequest);

                if (context.Response.StatusCode >= 400)
                {
                    FailedProcessCounter.WithLabels(AppDomain.CurrentDomain.FriendlyName).Inc();
                }

                if (context.Response.Headers != null && context.Response.Headers.ContainsKey(CONTENT_HEADER_NAME))
                {
                    var contentType = context.Response.Headers[CONTENT_HEADER_NAME].ToString();

                    //LOG content type as utf-8
                    if (contentType.Contains("utf-16", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var request = context.Request;
                        var rcType = request.ContentType;
                        var rcHeader = context.Request.Headers[CONTENT_HEADER_NAME];

                        _logAdaptor.LogError("UTF16, REQ:{rcType},{rcHeader}", rcType, rcHeader);
                    }
                }

                return Task.CompletedTask;
            });

            // Call the next delegate/middleware in the pipeline   

            return _next(context);
        }
    }
}