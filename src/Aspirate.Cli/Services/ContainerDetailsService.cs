namespace Aspirate.Cli.Services;
public class ContainerDetailsService(IProjectPropertyService propertyService, IAnsiConsole console) : IContainerDetailsService
{
    private static readonly StringBuilder _imageBuilder = new();

    public async Task<MsBuildContainerProperties> GetContainerDetails(
        string resourceName,
        Project project,
        string? containerRegistry,
        string? containerImageTag)
    {
        var containerPropertiesJson = await propertyService.GetProjectPropertiesAsync(
            project.Path,
            ContainerBuilderLiterals.ContainerRegistry,
            ContainerBuilderLiterals.ContainerRepository,
            ContainerBuilderLiterals.ContainerImageName,
            ContainerBuilderLiterals.ContainerImageTag);

        var msBuildProperties = JsonSerializer.Deserialize<MsBuildProperties<MsBuildContainerProperties>>(containerPropertiesJson ?? "{}");

        // Exit app if container registry is empty. We need it.
        EnsureContainerRegistryIsNotEmpty(msBuildProperties.Properties, project, containerRegistry);

        // Fallback to service name if image name is not provided from anywhere. (imageName is deprecated using repository like it says to).
        if (string.IsNullOrEmpty(msBuildProperties.Properties.ContainerRepository) && string.IsNullOrEmpty(msBuildProperties.Properties.ContainerImageName))
        {
            msBuildProperties.Properties.ContainerRepository = resourceName;
        }

        // Fallback to latest tag if tag not specified.
        HandleTag(msBuildProperties, containerImageTag);

        msBuildProperties.Properties.FullContainerImage = GetFullImage(msBuildProperties.Properties);

        return msBuildProperties.Properties;
    }

    private static string GetFullImage(MsBuildContainerProperties containerDetails)
    {
        _imageBuilder.Clear();

        HandleRegistry(containerDetails);

        HandleRepository(containerDetails);

        HandleImage(containerDetails);

        HandleTag(containerDetails);

        return _imageBuilder.ToString().Trim('/');
    }

    private static void HandleTag(MsBuildContainerProperties containerDetails) =>
        _imageBuilder.Append($":{containerDetails.ContainerImageTag}");

    private static void HandleImage(MsBuildContainerProperties containerDetails)
    {
        if (!string.IsNullOrEmpty(containerDetails.ContainerImageName))
        {
            _imageBuilder.Append($"/{containerDetails.ContainerImageName}");
        }
    }

    private static void HandleRepository(MsBuildContainerProperties containerDetails)
    {
        if (!string.IsNullOrEmpty(containerDetails.ContainerRepository))
        {
            _imageBuilder.Append($"/{containerDetails.ContainerRepository}");
        }
    }

    private static void HandleRegistry(MsBuildContainerProperties containerDetails) =>
        _imageBuilder.Append($"{containerDetails.ContainerRegistry}");

    private void EnsureContainerRegistryIsNotEmpty(
        MsBuildContainerProperties details,
        Project project,
        string? containerRegistry)
    {
        if (!string.IsNullOrEmpty(details.ContainerRegistry))
        {
            return;
        }

        // Use our custom fall-back value if it exists
        if (!string.IsNullOrEmpty(containerRegistry))
        {
            details.ContainerRegistry = containerRegistry;
            return;
        }

        console.MarkupLine($"[red bold]Required MSBuild property [blue]'ContainerRegistry'[/] not set in project [blue]'{project.Path}'. Cannot continue[/].[/]");
        throw new ActionCausesExitException(1);
    }

    private static void HandleTag(
        MsBuildProperties<MsBuildContainerProperties> msBuildProperties,
        string containerImageTag)
    {
        if (!string.IsNullOrEmpty(msBuildProperties.Properties.ContainerImageTag))
        {
            return;
        }

        // Use our custom fall-back value if it exists
        if (!string.IsNullOrEmpty(containerImageTag))
        {
            msBuildProperties.Properties.ContainerImageTag = containerImageTag;
            return;
        }

        msBuildProperties.Properties.ContainerImageTag = "latest";
    }
}
