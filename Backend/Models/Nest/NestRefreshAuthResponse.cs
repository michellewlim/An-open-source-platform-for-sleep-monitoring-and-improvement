using System.Text.Json.Serialization;

namespace Backend.Models.Nest;
public class NestRefreshAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string access_token { get; set; }

        [JsonPropertyName("expires_in")]
        public int expires_in { get; set; }

        [JsonPropertyName("scope")]
        public string scope { get; set; }

        [JsonPropertyName("token_type")]
        public string token_type { get; set; }

        public int user_id { get; set; }
    }