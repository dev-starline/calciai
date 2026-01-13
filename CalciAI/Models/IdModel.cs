using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    public class IdModel : IModel
    {

        [JsonPropertyName("id")] 
        public string Id { get; set; }

        public static IdModel Create(string id)
        {
            return new IdModel
            {
                Id = id
            };
        }
    }

    public class UrlModel : IModel
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        public static UrlModel Create(string url)
        {
            return new UrlModel
            {
                Url = url
            };
        }
    }

    public class NameModel : IModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public static NameModel Create(string name)
        {
            return new NameModel
            {
                Name = name
            };
        }
    }

    public class MessageModel : IModel
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        public static MessageModel Create(string message)
        {
            return new MessageModel
            {
                Message = message
            };
        }
    }

    public class IPModel : IModel
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        public static IPModel Create(string ip)
        {
            return new IPModel
            {
                Ip = ip
            };
        }
    }
}
