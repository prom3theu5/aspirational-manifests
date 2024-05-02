namespace Aspirate.Services.Implementations;
public class ContainerDetailsService(IProjectPropertyService propertyService, IAnsiConsole console) : IContainerDetailsService
{
    private static readonly StringBuilder _imageBuilder = new();

    public async Task<MsBuildContainerProperties> GetContainerDetails(
        string resourceName,
        ProjectResource projectResource,
        ContainerOptions options)
    {
        var containerPropertiesJson = await propertyService.GetProjectPropertiesAsync(
            projectResource.Path,
            ContainerBuilderLiterals.ContainerRegistry,
            ContainerBuilderLiterals.ContainerRepository,
            ContainerBuilderLiterals.ContainerImageName,
            ContainerBuilderLiterals.ContainerImageTag);

        var msBuildProperties = JsonSerializer.Deserialize<MsBuildProperties<MsBuildContainerProperties>>(containerPropertiesJson ?? "{}");

        // Exit app if container registry is empty. We need it.
        EnsureContainerRegistryIsNotEmpty(msBuildProperties.Properties, projectResource, options.Registry);

        // Fallback to service name if image name is not provided from anywhere. (imageName is deprecated using repository like it says to).
        if (string.IsNullOrEmpty(msBuildProperties.Properties.ContainerRepository) && string.IsNullOrEmpty(msBuildProperties.Properties.ContainerImageName))
        {
            msBuildProperties.Properties.ContainerRepository = resourceName;
        }

        // Fallback to latest tag if tag not specified.
        HandleTag(msBuildProperties, options.Tag);

        msBuildProperties.Properties.FullContainerImage = GetFullImage(msBuildProperties.Properties, options.Prefix);

        return msBuildProperties.Properties;
    }

    private static string GetFullImage(MsBuildContainerProperties containerDetails, string? containerPrefix)
    {
        _imageBuilder.Clear();

        HandleRegistry(containerDetails);

        HandleRepository(containerDetails, containerPrefix);

        HandleImage(containerDetails);

        HandleTag(containerDetails);

        return _imageBuilder.ToString().Trim('/');
    }

    private static void HandleTag(MsBuildContainerProperties containerDetails) =>
        _imageBuilder.Append($":{containerDetails.ContainerImageTag}");

    private static void HandleImage(MsBuildContainerProperties containerDetails)
    {
        if (HasImageName(containerDetails))
        {
            _imageBuilder.Append($"{containerDetails.ContainerImageName}");
        }
    }

    private static void HandleRepository(MsBuildContainerProperties containerDetails, string imagePrefix)
    {
        if (HasRepository(containerDetails))
        {
            if (!string.IsNullOrEmpty(imagePrefix))
            {
                _imageBuilder.Append($"{imagePrefix}/{containerDetails.ContainerRepository}");
            }
            else
            {
                _imageBuilder.Append($"{containerDetails.ContainerRepository}");
            }
        }

        if (HasImageName(containerDetails))
        {
            _imageBuilder.Append('/');
        }
    }

    private static void HandleRegistry(MsBuildContainerProperties containerDetails)
    {
        if (HasRegistry(containerDetails))
        {
            _imageBuilder.Append($"{containerDetails.ContainerRegistry}");
        }

        if (HasRepository(containerDetails))
        {
            _imageBuilder.Append('/');
        }
    }

    private void EnsureContainerRegistryIsNotEmpty(
        MsBuildContainerProperties details,
        ProjectResource projectResource,
        string? containerRegistry)
    {
        if (HasRegistry(details))
        {
            return;
        }

        // Use our custom fall-back value if it exists
        if (!string.IsNullOrEmpty(containerRegistry))
        {
            details.ContainerRegistry = containerRegistry;
            return;
        }
        //
        // console.MarkupLine($"[red bold]Required MSBuild property [blue]'ContainerRegistry'[/] not set in project [blue]'{project.Path}'. Cannot continue[/].[/]");
        // throw new ActionCausesExitException(1);
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


    private static bool HasImageName(MsBuildContainerProperties? containerDetails) => !string.IsNullOrEmpty(containerDetails?.ContainerImageName);
    private static bool HasRepository(MsBuildContainerProperties? containerDetails) => !string.IsNullOrEmpty(containerDetails?.ContainerRepository);
    private static bool HasRegistry(MsBuildContainerProperties? containerDetails) => !string.IsNullOrEmpty(containerDetails?.ContainerRegistry);
}
