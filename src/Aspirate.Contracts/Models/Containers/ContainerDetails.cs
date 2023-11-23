namespace Aspirate.Contracts.Models.Containers;

[ExcludeFromCodeCoverage]
public sealed class ContainerDetails(
    string resourceName,
    string? containerRegistry,
    string? containerRepository,
    string? containerImage,
    string? containerTag)
{
    public string? ContainerRegistry { get; } = containerRegistry;

    public string? ContainerRepository { get; } = containerRepository;

    public string? ContainerImage { get; } = containerImage;

    public string? ContainerTag { get; } = containerTag;

    public string? FullContainerImage { get; set; }
}
