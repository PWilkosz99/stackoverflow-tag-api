using System.Text.Json.Serialization;

namespace StackoverflowTagApi.Models
{
    public class Tag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("user_id")]
        public string? User_id { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("is_required")]
        public bool IsRequired { get; set; }
        [JsonPropertyName("is_moderator_only")]
        public bool IsModeratorOnly { get; set; }
        [JsonPropertyName("has_synonyms")]
        public bool has_synonyms { get; set; }
    }
}
