namespace Aspirate.Contracts.Models;

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

    public string GetFullImage()
    {
        var imageBuilder = new StringBuilder();

        if (ContainerRegistry is not null)
        {
            imageBuilder.Append($"{ContainerRegistry}/");
        }

        if (ContainerRepository is not null)
        {
            imageBuilder.Append($"{ContainerRepository}/");
        }

        imageBuilder.Append($"{ContainerImage ?? ResourceName}:{ContainerTag ?? "latest"}");

        return imageBuilder.ToString();
    }
}
