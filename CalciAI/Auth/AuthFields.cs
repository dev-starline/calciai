namespace CalciAI.Auth
{
    /// <summary>
    /// Move to app settings and use from configuration
    /// </summary>
    public sealed class AuthFields
    {
        private AuthFields()
        {

        }

        public const string AUTH_TOKEN = "X-AUTH-TOKEN";
        public const string COOKIE_SCHEME = "COOKIES";
        public const string CORS_POLICY = "CORS_POLICY";
        public const string API_TYPE = "API-TYPE";

        public const string AUTH_HEADER = "authorization";
        public const string AUTH_HEADER_TOKEN_PREFIX = "Bearer ";

        public const string OPERATOR_KEY = "X-OP-KEY";
        public const string OPERATOR_SECRET = "X-OP-SECRET";

        public const string PARTNER_KEY = "X-P-KEY";
        public const string PARTNER_SECRET = "X-P-SECRET";
        public const string PARTNER_CLAIM = "X-P-CLAIM";
    }
}
