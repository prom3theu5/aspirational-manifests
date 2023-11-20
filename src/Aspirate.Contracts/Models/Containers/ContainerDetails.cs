namespace Aspirate.Contracts.Models.Containers;

public sealed class ContainerDetails(
    string resourceName,
    string? containerRegistry,
    string? containerRepository,
    string? containerImage,
    string? containerTag)
{
    public string ResourceName { get; } = resourceName;
    public string? ContainerRegistry { get; } = containerRegistry;

    public string? ContainerRepository { get; } = containerRepository;

    public string? ContainerImage { get; } = containerImage;

    public string? ContainerTag { get; } = containerTag;
}
