using Aspirate.Shared.Models.AspireManifests.Components.V0.Container;

namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithInput
{
    string? Name { get; set; }
    Dictionary<string, Input>? Inputs { get; set; }
}
