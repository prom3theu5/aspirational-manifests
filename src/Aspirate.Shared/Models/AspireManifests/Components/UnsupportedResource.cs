namespace Aspirate.Shared.Models.AspireManifests.Components;

/// <summary>
/// A resource that is not yet supported by Aspirational Manifests.
/// </summary>
[ExcludeFromCodeCoverage]
public class UnsupportedResource : Resource
{
    /// <summary>
    /// The type of the unknown resource.
    /// </summary>
    public new const string Type = "unknown.resource.v0";
}
