namespace Aspirate.Contracts.Models.AspireManifests;

/// <summary>
/// A resource in a manifest.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class Resource
{
    /// <summary>
    /// The type of the resource.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// The environment variables for the project.
    /// </summary>
    [JsonPropertyName("env")]
    public Dictionary<string, string>? Env { get; set; }
}
