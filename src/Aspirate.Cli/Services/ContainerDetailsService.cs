namespace Aspirate.Cli.Services;
public class ContainerDetailsService(IProjectPropertyService propertyService) : IContainerDetailsService
{
    private static readonly char[] _filePathSeparator = ['\\', '/'];
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
        HandleEmptyContainerImageName(resourceName, containerProperties, project);

        return new(
            resourceName,
            containerProperties.Properties.ContainerRegistry,
            containerProperties.Properties.ContainerRepository,
            containerProperties.Properties.ContainerImage,
            containerProperties.Properties.ContainerImageTag);
    }

    public string GetDefaultImageName(Project project)
    {
        var pathSpan = project.Path.AsSpan();
        int lastSeparatorIndex = pathSpan.LastIndexOfAny(_filePathSeparator);
        int dotIndex = pathSpan.LastIndexOf('.');
        var fileNameSpan = pathSpan.Slice(lastSeparatorIndex + 1, dotIndex - lastSeparatorIndex - 1);

        return fileNameSpan.ToString().Kebaberize();
    }

    public string GetFullImage(ContainerDetails containerDetails, string defaultImageName)
    {
        _imageBuilder.Clear();

        if (!string.IsNullOrEmpty(containerDetails.ContainerRegistry))
        {
            _imageBuilder.Append($"{containerDetails.ContainerRegistry}");
            _imageBuilder.Append('/');
        }

        //if (!string.IsNullOrEmpty(containerDetails.ContainerRepository))
        //{
        //    _imageBuilder.Append($"{containerDetails.ContainerRepository}");
        //    _imageBuilder.Append('/');
        //}

        _imageBuilder.Append($"{(!string.IsNullOrEmpty(containerDetails.ContainerImage) ? containerDetails.ContainerImage : defaultImageName)}");
        _imageBuilder.Append(':');
        _imageBuilder.Append($"{(!string.IsNullOrEmpty(containerDetails.ContainerTag) ? containerDetails.ContainerTag : "latest")}");

        return _imageBuilder.ToString();
    }

    private void HandleEmptyContainerImageName(string serviceName, ContainerProperties? containerProperties, Project project)
    {
        if (string.IsNullOrEmpty(containerProperties.Properties.ContainerImage))
        {
            var defaultName = GetDefaultImageName(project);
            var useDefaultImageName = AskIfShouldUseDefaultImageName(serviceName, defaultName);
            containerProperties.Properties.ContainerImage = useDefaultImageName ? defaultName : AnsiConsole.Ask<string>("Enter the container image name");
        }
    }

    private static bool AskIfShouldUseDefaultImageName(string serviceName, string defaultName)
    {
        AnsiConsole.MarkupLine($"[yellow]No container image name was specified for project [green]{serviceName}[/].[/]");
        AnsiConsole.MarkupLine($"[yellow]The default image name will be used: [green]{defaultName}[/][/]");

        return AnsiConsole.Confirm("Do you want to use the default image name?");
    }
}
