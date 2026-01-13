using System.Text.Json.Serialization;

namespace CalciAI.Persistance.Entities
{
    public class FeedModule : Table
    {
        [JsonPropertyName("FeedModuleID")]
        public int FeedModuleID { get; set; }

        [JsonPropertyName("FeedModuleName")]
        public string FeedModuleName { get; set; }

        [JsonPropertyName("FeedModuleDisplayName")]
        public string FeedModuleDisplayName { get; set; }

        [JsonPropertyName("FeedModuleDesc")]
        public string FeedModuleDesc { get; set; }

        [JsonPropertyName("FeedModuleStatus")]
        public bool FeedModuleStatus { get; set; }
    }
}
