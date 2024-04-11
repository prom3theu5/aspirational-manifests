namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithBinding : IResource
{
    Dictionary<string, Binding>? Bindings { get; set; }
}
