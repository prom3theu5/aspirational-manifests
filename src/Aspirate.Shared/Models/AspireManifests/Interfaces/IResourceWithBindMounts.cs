namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithBindMounts : IResource
{
    List<BindMount>? BindMounts { get; set; }
}
