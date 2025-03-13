namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithEnvironmentalVariables : IResource
{
    Dictionary<string, string>? Env { get; set; }
}
