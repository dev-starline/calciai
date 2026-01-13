using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CalciAI.Web.LoggingMiddleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly ILoggerAdaptor<RequestResponseLoggingMiddleware> _logAdaptor;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
            _logAdaptor = new LoggerAdaptor<RequestResponseLoggingMiddleware>(_logger);
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            if (!(_logger.IsEnabled(LogLevel.Information) && new[] { "POST", "PUT", "DELETE" }.Contains(context.Request.Method)))
            {
                await _next(context);
                return;
            }

            context.Request.EnableBuffering();
            var startTime = DateTime.Now;
            var watcher = Stopwatch.StartNew();
            
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            _logAdaptor.LogInformation("Http Request Information:{NewLine}" +
                                   "Schema:{Scheme} " +
                                   "RemoteIp:{Ip} " +
                                   "Host: {Host} " +
                                   "Path: {Path} " +
                                   "QueryString: {QueryString} " +
                                   "Request Body: {ReadStreamInChunks}",
                                   Environment.NewLine,
                                   context.Request.Scheme,
                                   WebUtils.GetRemoteIp(context),
                                   context.Request.Host,
                                   context.Request.Path,
                                   context.Request.QueryString,
                                   ReadStreamInChunks(requestStream));

            context.Request.Body.Position = 0;

            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var endTime = DateTime.Now;
            var timeDiff = endTime - startTime;
            watcher.Stop();
            _logAdaptor.LogInformation("Http Response Information:{NewLine}" +
                                    "time:{Milliseconds}" +
                                   "Schema:{Scheme} " +
                                   "Host: {Host} " +
                                   "Path: {Path} " +
                                   "QueryString: {QueryString} " +
                                   "Response Body: {text}",
                                   Environment.NewLine,
                                   watcher.ElapsedMilliseconds,
                                   context.Request.Scheme,
                                   context.Request.Host,
                                   context.Request.Path,
                                   context.Request.QueryString,
                                   text);

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }
}
