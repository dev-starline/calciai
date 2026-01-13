using System;
using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    public class OperatorDetail : IModel
    {
        public const string KEY_CODE = "CODE";
        public const string KEY_SECRET = "SECRET";
        public const string KEY_PSECRET = "PSECRET";
        public const string KEY_AUTH = "AUTH";
        public const string KEY_IS_PRIMARY = "ISPRIMARY";
        public const string KEY_PRIMARY_DOMAIN = "PRIMARY_DOMAIN";
        public const string KEY_OTHER_DOMAIN = "OTHER_DOMAINS";
        public const string KEY_IS_ACTIVE = "ISACTIVE";

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("pSecret")]
        public string PartnerSecret { get; set; }

        [JsonPropertyName("authKey")]
        public string AuthKey { get; set; }

        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; set; }

        [JsonPropertyName("primaryDomain")]
        public string PrimaryDomain { get; set; }

        [JsonPropertyName("otherDomains")]
        public string[] OtherDomains { get; set; }

        [JsonPropertyName("operatorType")]
        public string OperatorType { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        public override string ToString()
        {
            return $"{KEY_CODE}:{Code}, " +
                $"{KEY_SECRET}:{Secret}, " +
                $"{KEY_PSECRET}:{PartnerSecret}, " +
                $"{KEY_AUTH}:{AuthKey}, " +
                $"{KEY_IS_PRIMARY}:{IsPrimary}, " +
                $"{KEY_PRIMARY_DOMAIN}:{PrimaryDomain}, " +
                $"{KEY_OTHER_DOMAIN}:{string.Join(',', OtherDomains)}, " +
                $"{KEY_IS_ACTIVE}:{IsActive}";
        }
    }
}
