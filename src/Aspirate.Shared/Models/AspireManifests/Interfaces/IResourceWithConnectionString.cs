namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithConnectionString : IResource
{
    string? ConnectionString { get; set; }
}
