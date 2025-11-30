using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

public class OllamaResponse
{
    [JsonPropertyName("embedding")]
    public required float[] Embedding { get; set; }
}