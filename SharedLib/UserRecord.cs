using System.Text.Json.Serialization;

namespace Shared;

public class UserRecord {
    [JsonPropertyName("id")]
    public required string Id {get; set;}
    [JsonPropertyName("name")]
    public required string Name {get; set;}
    [JsonPropertyName("description")]
    public required string Description {get; set;}
    public float[]? Embedding {get; set;}
}