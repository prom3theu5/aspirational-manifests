namespace Aspirational.Manifests.Models.Components.V0;

/// <summary>
/// A project within an aspire manifest.
/// </summary>
public class Project : Resource
{
    /// <summary>
    /// The path to the project.
    /// </summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }
    
    /// <summary>
    /// The environment variables for the project.
    /// </summary>
    [JsonPropertyName("env")]
    public Dictionary<string, string>? Env { get; set; }
    
    /// <summary>
    /// The bindings for the project.
    /// </summary>
    [JsonPropertyName("bindings")]
    public Dictionary<string, Binding>? Bindings { get; set; }
}