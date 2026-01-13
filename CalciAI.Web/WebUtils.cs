using CalciAI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace CalciAI.Web
{
    public static class WebUtils
    {
        public static string GetRemoteIp(HttpContext context)
        {
            if(context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return context.Request.Headers["X-Forwarded-For"].ToString().Split(",")[0];
            }
            
            return context.Connection.RemoteIpAddress.ToString();
        }

        public static DeviceType GetDeviceType(HttpContext context)
        {
            var strToReturn = $"User agent: {context.Request.Headers["User-Agent"]}";
            return strToReturn.ToLower().Contains("mobi") ? DeviceType.MOBILE : DeviceType.DESKTOP;
        }

        public static string GetHost(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Host", out StringValues host))
            {
                return null;
            }

            var hostname = host.ToString();

            var domainName = hostname.Split(":")[0].ToString();

            if (domainName.StartsWith("api.") || domainName.StartsWith("s2s."))
            {
                return domainName[4..];
            }

            if (domainName.StartsWith("wskt."))
            {
                return domainName[5..];
            }

            if (domainName.StartsWith("tradex."))
            {
                return domainName[7..];
            }

            var domainArtifcates = domainName.Split(".");

            if (domainName.StartsWith("admin.") || domainName.StartsWith("dealer.") || domainName.StartsWith("client."))
            {
                var domainData = domainArtifcates.Skip(1).ToArray();
                return string.Join(".", domainData);
            }

            if (domainArtifcates.Length >= 3)
            {
                var domainData = domainArtifcates.ToArray();
                return string.Join(".", domainData);
            }

            return domainName;
        }

        public static string GetOrigin(HttpContext context)
        {
            var origin = context.Request.Headers["Origin"].ToString();

            var domainName = origin.Split(":")[0].ToString();

            var domainArtifcates = origin.Split(".");

            if (domainName.StartsWith("admin.") || domainName.StartsWith("dealer.") || domainName.StartsWith("client."))
            {
                var domainData = domainArtifcates.Skip(1).ToArray();
                return string.Join(".", domainData);
            }

            if (domainArtifcates.Length >= 3)
            {
                var domainData = domainArtifcates.ToArray();
                return string.Join(".", domainData);
            }

            return origin;
        }
    }
}
