namespace Aspirate.Shared.Models.AspireManifests.Components.V0;

/// <summary>
/// A project within an aspire manifest.
/// </summary>
[ExcludeFromCodeCoverage]
public class ProjectResource : Resource, IResourceWithBinding
{
    /// <summary>
    /// The path to the project.
    /// </summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>
    /// The bindings for the project.
    /// </summary>
    [JsonPropertyName("bindings")]
    public Dictionary<string, Binding>? Bindings { get; set; }
}
