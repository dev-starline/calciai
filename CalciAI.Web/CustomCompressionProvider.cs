using Microsoft.AspNetCore.ResponseCompression;
using System.IO;

namespace CalciAI.Web
{
    public class CustomCompressionProvider : ICompressionProvider
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-6.0
        /// provide custom encoding, and will give response if accpet encoding header have encoding name
        /// can be used in future in security
        /// </summary>
        public string EncodingName => "CalciAI";
        public bool SupportsFlush => true;

        public Stream CreateStream(Stream outputStream)
        {
            // Create a custom compression stream wrapper here
            return outputStream;
        }
    }
}