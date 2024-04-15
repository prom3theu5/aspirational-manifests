namespace Aspirate.Shared.Models.AspireManifests.Components.V0;

/// <summary>
/// A binding for a project.
/// </summary>
[ExcludeFromCodeCoverage]
public class Binding
{
    /// <summary>
    /// The schema of the binding.
    /// </summary>
    [JsonPropertyName("schema")]
    public string? Scheme { get; set; } = "http";

    /// <summary>
    /// The protocol of the binding.
    /// </summary>
    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; } = "tcp";

    /// <summary>
    /// The transport for the binding.
    /// </summary>
    [JsonPropertyName("transport")]
    public string? Transport { get; set; }

    /// <summary>
    /// The Container Port for the binding.
    /// </summary>
    [JsonPropertyName("port")]
    public int? Port { get; set; }

    /// <summary>
    /// The Container Port for the binding.
    /// </summary>
    [JsonPropertyName("targetPort")]
    public int? TargetPort { get; set; }

    /// <summary>
    /// Is External.
    /// </summary>
    [JsonPropertyName("external")]
    public bool External { get; set; }
}
