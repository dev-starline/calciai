using CalciAI.Models;
using System.Text.Json.Serialization;

namespace CalciAI.Persistance.Entities
{
    public class MongoSetting : ISetting
    {
        [JsonPropertyName("sAypUdyOe/IBrdX9sNY5OWD+zM+z7uvc95yQfvmZnzk=")] // connectionString
        public string ConnectionString { get; set; }

        [JsonPropertyName("1ZBzKh1yy8t1pNQuTbGICg==")] //database
        public string Database { get; set; }
    }
}
