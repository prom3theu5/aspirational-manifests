namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithInput
{
    string? Name { get; set; }
    Dictionary<string, Input>? Inputs { get; set; }
}
