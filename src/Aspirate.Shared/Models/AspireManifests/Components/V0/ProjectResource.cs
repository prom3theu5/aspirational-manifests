namespace Aspirate.Shared.Models.AspireManifests.Components.V0;

/// <summary>
/// A project within an aspire manifest.
/// </summary>
[ExcludeFromCodeCoverage]
public class ProjectResource : Resource, IResourceWithBinding, IResourceWithAnnotations, IResourceWithEnvironmentalVariables, IResourceWithArgs
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
    public Dictionary<string, Binding>? Bindings { get; set; } = [];

    [JsonPropertyName("annotations")]
    public Dictionary<string, string>? Annotations { get; set; } = [];

    [JsonPropertyName("env")]
    public Dictionary<string, string>? Env { get; set; } = [];

    [JsonPropertyName("args")]
    public List<string>? Args { get; set; } = [];
}
