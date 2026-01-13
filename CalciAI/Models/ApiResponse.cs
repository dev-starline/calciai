using System.Net;

namespace CalciAI.Models
{
    public class ApiResponse
    {
        public byte[] Response { get; set; }

        public bool IsSuccess => (int)StatusCode is >= 200 and < 400;

        public HttpStatusCode StatusCode { get; set; }
    }
}
