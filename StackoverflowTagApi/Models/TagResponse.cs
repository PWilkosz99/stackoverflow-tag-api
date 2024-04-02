using System.Text.Json.Serialization;

namespace StackoverflowTagApi.Models
{
    public class TagResponse
    {
        [JsonPropertyName("items")]
        public List<Tag> Items { get; set; }
    }
}
