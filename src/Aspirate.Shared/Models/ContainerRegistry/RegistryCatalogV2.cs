namespace Aspirate.Shared.Models.ContainerRegistry;

public class RegistryCatalogV2(IReadOnlyCollection<string> repositories)
{
    [JsonPropertyName("repositories")]
    public IReadOnlyCollection<string> Repositories { get; } = repositories;
}
