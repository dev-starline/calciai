using CalciAI.Auth;
using CalciAI.Persistance;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CalciAI.Web.AuthMiddleware
{
    /// <summary>
    /// https://stackoverflow.com/questions/20254796/why-is-my-claimsidentity-isauthenticated-always-false-for-web-api-authorize-fil
    /// </summary>
    public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
    {
        public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Options.AuthTypes.Contains("S2S") && CheckS2S(out var s2sIdentity))
            {
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(s2sIdentity), Options.Scheme);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            if (Options.AuthTypes.Contains("JWT") && CheckJWT(Options.IsHostOrigin, out var jwtIdentity))
            {
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(jwtIdentity), Options.Scheme);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail($"All auth schemes failed. Path:{Request.Path}, " +
                $"RemoteIP: {WebUtils.GetRemoteIp(Request.HttpContext)}, " +
                $"Host: { WebUtils.GetHost(Request.HttpContext)}"));
        }

        private bool CheckS2S(out ClaimsIdentity identity)
        {
            identity = null;

            if (!Request.Headers.TryGetValue(AuthFields.OPERATOR_KEY, out var clientKey))
            {
                Logger.LogTrace("Error S2S AUTH FAILED. ClientKey not supplied");
                return false;
            }

            if (!Request.Headers.TryGetValue(AuthFields.OPERATOR_SECRET, out var clientSecret))
            {
                Logger.LogTrace("Error S2S AUTH FAILED. ClientSecret not supplied");
                return false;
            }

            var requestIP = WebUtils.GetRemoteIp(Request.HttpContext);

            //NOTE: We can add Source IP for more security
            var isValidHeader = OperatorProvider.ValidateOperator(clientKey, clientSecret);

            if (Request.Path == "/manage/license/testconnection" && Request.Method == "GET")
            {
                isValidHeader = true;
            }
            else if (Request.Path == "/manage/license" && (Request.Method == "POST" || Request.Method == "PUT"))
            {
                isValidHeader = true;
            }

            if (!isValidHeader)
            {
                Logger.LogTrace("Error S2S AUTH FAILED. Invalid Key Secret Pair: {clientKey}, {requestIP}, {clientSecret}", clientKey, requestIP, clientSecret);
                return false;
            }

            identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Sid, clientKey),
                new Claim(ClaimTypes.Role, Roles.S2S),
                new Claim(ClaimTypes.GroupSid, clientKey)
            }, "S2S");

            return true;
        }

        private bool CheckJWT(bool isHostOrigin, out ClaimsIdentity identity)
        {
            identity = null;

            var host = isHostOrigin ? WebUtils.GetOrigin(Request.HttpContext) : WebUtils.GetHost(Request.HttpContext);

            if (string.IsNullOrEmpty(host))
            {
                Logger.LogTrace("JWT AUTH FAILED. Origin not present");
                return false;
            }

            string token = null;

            if (Request.Headers.ContainsKey(AuthFields.AUTH_HEADER))
            {
                token = ((string)Request.Headers[AuthFields.AUTH_HEADER]).Replace(AuthFields.AUTH_HEADER_TOKEN_PREFIX, "");
            }
            else if (Request.Cookies.ContainsKey(AuthFields.AUTH_TOKEN))
            {
                token = Request.Cookies[AuthFields.AUTH_TOKEN];
            }

            if (token == null)
            {
                Logger.LogTrace("JWT AUTH FAILED. Invalid Key Secret Pair: X-AUTH-TOKEN or AUTH_HEADER not present");
                return false;
            }

            var secret = SecretKey.secret;// OperatorProvider.GetSecretByDomain(host);

            string ApiType = "";

            if (Request.Headers.ContainsKey(AuthFields.API_TYPE))
            {
                ApiType = ((string)Request.Headers[AuthFields.API_TYPE]);
            }

            var response = AuthUtil.DecodeJwtToken(token, secret);

            if (!response.IsSuccess)
            {
                Logger.LogTrace("JWT AUTH FAILED. {host} - {token} not authorized", host, token);
                return false;
            }

            var tokenData = response.Data;

            identity = new ClaimsIdentity(new List<Claim>
                {

                    new Claim(ClaimTypes.Sid, tokenData.Username),
                    new Claim(ClaimTypes.Role, tokenData.Role),
                    new Claim(ClaimTypes.Expiration, tokenData.Expiration.ToString()),
                }, "JWT");

            return true;
        }
    }
}

