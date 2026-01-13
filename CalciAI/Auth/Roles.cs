namespace CalciAI.Auth
{
    public sealed class Roles
    {
        private Roles()
        {

        }

        /// <summary>
        /// Server to server role that will check api with key and incoming IP
        /// </summary>
        public const string Local = "Local";
        public const string S2S = "S2S";
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Supervisor = "Supervisor";
        public const string Trader = "Trader";
        public const string Analyst = "Analyst";
        public const string SuperMaster = "SuperMaster";
        public const string SubSuperMaster = "SubSuperMaster";
        public const string Master = "Master";
        public const string Player = "Client";
    }
}
