namespace Aspirate.Shared.Models.ContainerRegistry;

public class RegistryTagListV2(string name, IReadOnlyCollection<string> tags)
{
    [JsonPropertyName("name")]
    public string Name { get; } = name;

    [JsonPropertyName("tags")]
    public IReadOnlyCollection<string> Tags { get; } = tags;
}
