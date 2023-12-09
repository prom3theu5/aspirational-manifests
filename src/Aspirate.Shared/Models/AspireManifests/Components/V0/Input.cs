namespace Aspirate.Shared.Models.AspireManifests.Components.V0;

[ExcludeFromCodeCoverage]
public class Input
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("secret")]
    public bool Secret { get; set; }

    [JsonPropertyName("default")]
    public Default? Default { get; set; }
}
