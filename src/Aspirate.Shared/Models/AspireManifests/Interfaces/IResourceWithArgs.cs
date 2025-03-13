namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithArgs : IResource
{
    List<string>? Args { get; set; }
}
