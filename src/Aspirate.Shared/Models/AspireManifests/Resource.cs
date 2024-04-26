namespace Aspirate.Shared.Models.AspireManifests;

/// <summary>
/// A resource in a manifest.
/// </summary>
[ExcludeFromCodeCoverage]
[JsonPolymorphic]
[JsonDerivedType(typeof(ProjectResource))]
[JsonDerivedType(typeof(DockerfileResource))]
[JsonDerivedType(typeof(ContainerResource))]
[JsonDerivedType(typeof(DaprResource))]
[JsonDerivedType(typeof(DaprComponentResource))]
[JsonDerivedType(typeof(ParameterResource))]
[JsonDerivedType(typeof(ValueResource))]
public abstract class Resource : IResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// The type of the resource.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;
}
