namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Dapr;

public sealed class DaprComponentResource : Resource
{
    [JsonPropertyName("daprComponent")]
    public InnerDaprComponent? DaprComponentProperty { get; set; }
}
