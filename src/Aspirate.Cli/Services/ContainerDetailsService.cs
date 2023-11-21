namespace Aspirate.Cli.Services;
public class ContainerDetailsService(IProjectPropertyService propertyService) : IContainerDetailsService
{
    private static readonly StringBuilder _imageBuilder = new();

    public async Task<ContainerDetails> GetContainerDetails(string resourceName, Project project)
    {
        var containerPropertiesJson = await propertyService.GetProjectPropertiesAsync(
            project.Path,
            ContainerBuilderLiterals.ContainerRegistry,
            ContainerBuilderLiterals.ContainerRepository,
            ContainerBuilderLiterals.ContainerImageName,
            ContainerBuilderLiterals.ContainerImageTag);

        var containerProperties = JsonSerializer.Deserialize<ContainerProperties>(containerPropertiesJson ?? "{}");

        return new(
            resourceName,
            containerProperties.Properties.ContainerRegistry,
            containerProperties.Properties.ContainerRepository,
            containerProperties.Properties.ContainerImage,
            containerProperties.Properties.ContainerImageTag);
    }

    public string GetFullImage(ContainerDetails containerDetails, Project project)
    {
        _imageBuilder.Clear();

        HandleRegistry(containerDetails);

        HandleRepository(containerDetails);

        HandleImage(containerDetails);

        HandleTag(containerDetails);

        return _imageBuilder.ToString();
    }

    private static void HandleTag(ContainerDetails containerDetails)
    {
        if (!string.IsNullOrEmpty(containerDetails.ContainerTag))
        {
            _imageBuilder.Append($":{containerDetails.ContainerTag}");
            return;
        }

        _imageBuilder.Append(":latest");
    }

    private static void HandleImage(ContainerDetails containerDetails)
    {
        if (!string.IsNullOrEmpty(containerDetails.ContainerImage))
        {
            _imageBuilder.Append($"/{containerDetails.ContainerImage}");
        }
    }

    private static void HandleRepository(ContainerDetails containerDetails)
    {
        if (!string.IsNullOrEmpty(containerDetails.ContainerRepository))
        {
            _imageBuilder.Append($"/{containerDetails.ContainerRepository}");
        }
    }

    private static void HandleRegistry(ContainerDetails containerDetails)
    {
        if (!string.IsNullOrEmpty(containerDetails.ContainerRegistry))
        {
            _imageBuilder.Append($"{containerDetails.ContainerRegistry}");
        }
    }
}
