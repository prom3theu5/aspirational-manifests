namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithInput
{
    Dictionary<string, Input>? Inputs { get; set; }
}
