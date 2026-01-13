using Prometheus;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CalciAI.Net
{
    public class RetryPolicyHandler : DelegatingHandler
    {
        private static readonly Counter RequestCountByMethod = Metrics.CreateCounter("api_total", "Number of api sent", new CounterConfiguration
        {
            LabelNames = new[] { "Key" }
        });

        private static readonly Histogram DurationHistogram = Metrics.CreateHistogram("api_duration", "Histogram of api durations.", new HistogramConfiguration
        {
            LabelNames = new[] { "Key" }
        });

        private static readonly Counter FailedProcessCounter = Metrics.CreateCounter("api_failed_total", "Number of failed api.", new CounterConfiguration
        {
            LabelNames = new[] { "Key" }
        });

        private readonly string _key;

        public RetryPolicyHandler(string key)
        {
            _key = key;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var watch = new Stopwatch();
            watch.Start();

            while (true)
            {
                try
                {
                    // base.SendAsync calls the inner handler
                    var response = await base.SendAsync(request, cancellationToken);

                    watch.Stop();

                    RequestCountByMethod.WithLabels(_key).Inc();

                    DurationHistogram.WithLabels(_key).Observe(watch.ElapsedMilliseconds);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        FailedProcessCounter.WithLabels(_key).Inc();
                    }

                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        // 503 Service Unavailable
                        // Wait a bit and try again later
                        await Task.Delay(5000, cancellationToken);
                        continue;
                    }

                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        // 429 Too many requests
                        // Wait a bit and try again later
                        await Task.Delay(1000, cancellationToken);
                        continue;
                    }

                    return response;
                }
                catch (Exception ex) when (IsNetworkError(ex))
                {
                    // Network error
                    // Wait a bit and try again later
                    await Task.Delay(2000, cancellationToken);
                }
            }
        }

        private static bool IsNetworkError(Exception ex)
        {
            // Check if it's a network error
            if (ex is SocketException or IOException)
            {
                return true;
            }

            if (ex.InnerException != null)
            {
                return IsNetworkError(ex.InnerException);
            }

            return false;
        }
    }
}