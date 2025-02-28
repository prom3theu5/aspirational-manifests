namespace Aspirate.Shared.Models.ContainerRegistry;

public class ImageManifestV2(
    string mediaType,
    int size,
    string digest)
    : ImageManifestV2Base(mediaType)
{
    [JsonPropertyName("size")]
    public int Size { get; } = size;

    [JsonPropertyName("digest")]
    public string Digest { get; } = digest;
}
