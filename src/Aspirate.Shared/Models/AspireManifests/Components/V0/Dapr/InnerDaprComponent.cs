namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Dapr;

public sealed class InnerDaprComponent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }
}
