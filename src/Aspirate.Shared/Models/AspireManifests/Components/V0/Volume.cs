namespace Aspirate.Shared.Models.AspireManifests.Components.V0;

public class Volume
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("target")]
    public string? Target { get; set; }

    [JsonPropertyName("readOnly")]
    public bool ReadOnly { get; set; }
}
