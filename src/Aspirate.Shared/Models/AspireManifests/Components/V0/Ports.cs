namespace Aspirate.Shared.Models.AspireManifests.Components.V0;

public class Ports
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("internalPort")]
    public int InternalPort { get; set; }

    [JsonPropertyName("externalPort")]
    public int ExternalPort { get; set; }
}
