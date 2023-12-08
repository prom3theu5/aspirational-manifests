namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithBinding
{
    Dictionary<string, Binding>? Bindings { get; set; }
}
