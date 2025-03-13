namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Container;

public class ContainerResource : ContainerResourceBase
{
    [JsonPropertyName("image")]
    public required string Image { get; set; }
}
