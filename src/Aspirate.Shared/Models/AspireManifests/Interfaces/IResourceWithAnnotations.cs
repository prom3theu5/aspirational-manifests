namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithAnnotations : IResource
{
    Dictionary<string, string>? Annotations { get; set; }
}
