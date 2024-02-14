namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Azure;

[ExcludeFromCodeCoverage]
public class AzureStorageBlobResource : Resource, IResourceWithParent
{
    [JsonPropertyName("parent")]
    public string? Parent { get; set; }
}
