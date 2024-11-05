namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Container;

public class ContainerResourceV1Build
{
    [JsonPropertyName("context")]
    public string? Context { get; set; }

    [JsonPropertyName("dockerfile")]
    public string? Dockerfile { get; set; }
}

public class ContainerResourceV1 : ContainerResource
{
    [JsonPropertyName("build")]
    public ContainerResourceV1Build? Build { get; set; }
}
