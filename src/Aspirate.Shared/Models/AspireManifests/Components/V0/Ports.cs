namespace Aspirate.Shared.Models.AspireManifests.Components.V0;

public class Ports
{
    public required string Name { get; set; }

    public int InternalPort { get; set; }

    public int ExternalPort { get; set; }
}
