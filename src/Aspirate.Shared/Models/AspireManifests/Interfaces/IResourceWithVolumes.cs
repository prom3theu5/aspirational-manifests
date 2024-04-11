using Volume = Aspirate.Shared.Models.AspireManifests.Components.V0.Volume;

namespace Aspirate.Shared.Models.AspireManifests.Interfaces;

public interface IResourceWithVolumes
{
    List<Volume>? Volumes { get; set; }
}
