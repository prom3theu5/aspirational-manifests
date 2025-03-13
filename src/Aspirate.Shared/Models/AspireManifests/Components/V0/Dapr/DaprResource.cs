namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Dapr;

public sealed class DaprResource : Resource
{
    [JsonPropertyName("dapr")]
    public Metadata? Metadata { get; set; }
}
