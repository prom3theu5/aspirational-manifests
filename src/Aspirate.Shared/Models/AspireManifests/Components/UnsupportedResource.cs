namespace Aspirate.Shared.Models.AspireManifests.Components;

/// <summary>
/// A resource that is not yet supported by Aspirational Manifests.
/// </summary>
[ExcludeFromCodeCoverage]
public class UnsupportedResource : Resource
{
    public UnsupportedResource() => Type = "unknown.resource.v0";
}
