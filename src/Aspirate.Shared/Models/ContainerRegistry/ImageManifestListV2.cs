namespace Aspirate.Shared.Models.ContainerRegistry;

public class ImageManifestListV2(
    int schemaVersion,
    string mediaType,
    ImageManifestV2 config,
    IReadOnlyCollection<ImageManifestV2> layers)
    : ImageManifestV2Base(mediaType)
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; } = schemaVersion;

    [JsonPropertyName("config")]
    public ImageManifestV2 Config { get; } = config;

    [JsonPropertyName("layers")]
    public IReadOnlyCollection<ImageManifestV2> Layers { get; } = layers;
}
