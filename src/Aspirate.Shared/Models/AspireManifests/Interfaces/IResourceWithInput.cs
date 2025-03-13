namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithInput : IResource
{
    Dictionary<string, ParameterInput>? Inputs { get; set; }
}
