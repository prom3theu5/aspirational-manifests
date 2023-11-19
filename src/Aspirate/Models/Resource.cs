namespace Aspirate.Models;

/// <summary>
/// A resource in a manifest.
/// </summary>
public abstract class Resource
{
    /// <summary>
    /// The type of the resource.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}