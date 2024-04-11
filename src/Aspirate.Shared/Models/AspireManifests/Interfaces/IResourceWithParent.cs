namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithParent : IResource
{
    string? Parent { get; set; }
}
