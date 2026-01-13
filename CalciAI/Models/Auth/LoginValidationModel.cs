using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    public class LoginValidationModel : IModel
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("userPassword")]
        public string UserPassword { get; set; }

        [JsonPropertyName("operatorId")]
        public string OperatorID { get; set; }

        [JsonPropertyName("licenseId")]
        public int LicenseID { get; set; }

        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("requestedPortal")]
        public string RequestedPortal { get; set; }
    }
}