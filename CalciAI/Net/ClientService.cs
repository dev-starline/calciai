using CalciAI.Models;
using CalciAI.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalciAI.Net
{
    public class ClientService
    {
        private static readonly Lazy<SocketsHttpHandler> _handler = new(() =>
        {
            var sslOptions = new SslClientAuthenticationOptions
            {
                // Leave certs unvalidated for debugging
                RemoteCertificateValidationCallback = delegate { return true; },
            };

            var handler = new SocketsHttpHandler()
            {
                SslOptions = sslOptions,
                PooledConnectionLifetime = TimeSpan.FromHours(1),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                MaxConnectionsPerServer = 50
            };

            return handler;
        });

        protected readonly ILogger Logger;
        protected readonly HttpClientConfig Configuration;

        public ClientService(ILogger logger, string serviceName)
        {
            Logger = logger;
            Configuration = FileConfigProvider.Load<HttpClientConfig>(serviceName);
        }

        public ClientService(ILogger logger, HttpClientConfig config)
        {
            Logger = logger;
            Configuration = config;
        }

        private HttpClient InitClient()
        {
            var pipeline = new RetryPolicyHandler(Configuration.Name)
            {
                InnerHandler = _handler.Value
            };

            if (Configuration.TimeOut == 0)
            {
                Configuration.TimeOut = 5;
                Logger.LogTrace("Time out not found in {Name}", Configuration.Name);
            }

            var client = new HttpClient(pipeline, disposeHandler: false)
            {
                Timeout = TimeSpan.FromSeconds(Configuration.TimeOut)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            AppendHeaders(client.DefaultRequestHeaders);

            return client;
        }

        /// <summary>
        /// Append base headers from configuration
        /// </summary>
        /// <param name="headers"></param>
        public virtual void AppendHeaders(HttpRequestHeaders headers)
        {
            //Override this method to append custom headers.
            foreach (var item in Configuration.BaseHeaders)
            {
                headers.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Before Send Request
        /// </summary>
        /// <param name="headers"></param>
        public virtual void BeforeRequestSent(HttpRequestMessage request, string payload)
        {

        }

        public async Task<ProcessResult<T, E>> InvokeAsync<T, E>(HttpMethod methodType, string path, object postData = null) where T : IModel where E : IException
        {
            var result = await InvokeAsync(methodType, path, postData);

            try
            {
                if (result.IsSuccess)
                {
                    return ProcessResult<T, E>.Success(result.Response.Length > 0 ? JsonSerializer.Deserialize<T>(result.Response, SerializationExtension.DefaultOptions) : default);
                }
                else
                {
                    return ProcessResult<T, E>.Fail(result.Response.Length > 0 ? JsonSerializer.Deserialize<E>(result.Response, SerializationExtension.DefaultOptions) : default);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("HTTP:{IsSuccess} - {Message} : payload: {Payload}", result.IsSuccess, ex.Message, Encoding.UTF8.GetString(result.Response, 0, result.Response.Length));
                return ProcessResult<T, E>.Fail("Response", ex.Message);
            }
        }

        public async Task<ProcessResult<T>> InvokeAsync<T>(HttpMethod methodType, string path, object postData = null) where T : IModel
        {
            var result = await InvokeAsync(methodType, path, postData);

            try
            {
                if (result.IsSuccess)
                {
                    if (result.Response.Length > 0)
                    {
                        return ProcessResult<T>.Success(JsonSerializer.Deserialize<T>(result.Response, SerializationExtension.DefaultOptions));
                    }
                    else
                    {
                        return ProcessResult<T>.Success(default);
                    }
                }
                else
                {
                    if (result.Response.Length > 0)
                    {
                        return ProcessResult<T>.Fail(JsonSerializer.Deserialize<Error>(result.Response, SerializationExtension.DefaultOptions));
                    }
                    else
                    {
                        return ProcessResult<T>.Fail(result.StatusCode.ToString(), "Error processing request");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("HTTP:{IsSuccess} - {Message} : payload: {Payload}", result.IsSuccess, ex.Message, Encoding.UTF8.GetString(result.Response, 0, result.Response.Length));
                return ProcessResult<T>.Fail("Response", ex.Message);
            }
        }

        public async Task<ApiResponse> InvokeAsync(HttpMethod methodType, string path, object postData = null)
        {
            var client = InitClient();

            try
            {
                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();

                var request = new HttpRequestMessage
                {
                    Method = methodType,
                    RequestUri = new Uri($"{Configuration.BaseUrl}{path}")
                };

                var payload = string.Empty;

                if ((methodType == HttpMethod.Post || methodType == HttpMethod.Put) && postData != null)
                {
                    payload = JsonSerializer.Serialize(postData);
                    request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                }

                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace("REQUEST: Method: {MethodType}, URI: {BaseUrl}{Path}, Payload: {Payload}", methodType, Configuration.BaseUrl, path, payload);
                }

                BeforeRequestSent(request, payload);

                var httpResponseMessage = await client.SendAsync(request);
                var resultBytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();

                watch.Stop();

                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace("RESPONSE: URL: {RequestUri} Code: {StatusCode}, Body: {Body}, Time: {ElapsedMilliseconds}", request.RequestUri, httpResponseMessage.StatusCode, Encoding.UTF8.GetString(resultBytes, 0, resultBytes.Length), watch.ElapsedMilliseconds);
                }
                else if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Logger.LogError("RESPONSE: URL: {RequestUri} Code: {StatusCode}, Body: {Body}, Time: {ElapsedMilliseconds}", request.RequestUri, httpResponseMessage.StatusCode, Encoding.UTF8.GetString(resultBytes, 0, resultBytes.Length), watch.ElapsedMilliseconds);
                }
                else if (watch.ElapsedMilliseconds > 5000)
                {
                    Logger.LogWarning("RESPONSE: URL: {RequestUri} Code: {StatusCode}, Body: {Body}, Time: {ElapsedMilliseconds}", request.RequestUri, httpResponseMessage.StatusCode, Encoding.UTF8.GetString(resultBytes, 0, resultBytes.Length), watch.ElapsedMilliseconds);
                }

                return new ApiResponse
                {
                    Response = resultBytes,
                    StatusCode = httpResponseMessage.StatusCode
                };
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException or TimeoutException or TaskCanceledException) 
                {
                    Logger.LogError("Error in Invoke Method:{path}-ResponseCode:{Message}", path, ex.Message);

                    return new ApiResponse
                    {
                        Response = Array.Empty<byte>()
                    };
                }

                Logger.LogError("Error Type: {Type} in Invoke Method:{path}-ResponseCode:{Message}", ex.GetType(), path, ex.Message);

                throw;
            }
        }
    }
}
