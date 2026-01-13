using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace CalciAI.Web.AuthMiddleware
{
    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "custom auth";

        public string Scheme => DefaultScheme;

        public StringValues AuthTypes { get; set; }

        public bool IsHostOrigin { get; set; }
    }
}

