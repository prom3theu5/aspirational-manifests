namespace Aspirate.Shared.Models.AspireManifests.Components.V1.Container;

public class ContainerV1Resource : ContainerResourceBase
{
    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("build")]
    public Build? Build { get; set; }
}
