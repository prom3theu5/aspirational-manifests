namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Parameters;

public class ValueResource : Resource
{
    [JsonExtensionData]
    public Dictionary<string, object> Values { get; init; } = [];
}
