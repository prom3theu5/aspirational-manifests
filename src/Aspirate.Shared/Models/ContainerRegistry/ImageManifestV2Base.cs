namespace Aspirate.Shared.Models.ContainerRegistry;

public abstract class ImageManifestV2Base(string mediaType)
{
    [JsonPropertyName("mediaType")]
    public string MediaType { get; } = mediaType;
}
