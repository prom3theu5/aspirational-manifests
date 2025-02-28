namespace Aspirate.Shared.Models.ContainerRegistry;

public class ImageV1(
    DateTimeOffset created,
    string author,
    string architecture,
    string os)
{
    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; } = created;

    [JsonPropertyName("author")]
    public string Author { get; } = author;

    [JsonPropertyName("architecture")]
    public string Architecture { get; } = architecture;

    [JsonPropertyName("os")]
    public string OS { get; } = os;
}
