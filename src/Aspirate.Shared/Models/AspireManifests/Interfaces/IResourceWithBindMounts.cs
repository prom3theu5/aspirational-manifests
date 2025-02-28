namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithBindMounts
{
    List<BindMount>? BindMounts { get; set; }
}
