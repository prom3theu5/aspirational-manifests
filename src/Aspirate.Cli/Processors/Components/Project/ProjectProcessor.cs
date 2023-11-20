using AspireProject = Aspirate.Contracts.Models.AspireManifests.Components.V0.Project;

namespace Aspirate.Cli.Processors.Components.Project;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public partial class ProjectProcessor(
    IFileSystem fileSystem,
    ILogger<ProjectProcessor> logger,
    IProjectPropertyService propertyService)
        : BaseProcessor<ProjectTemplateData>(fileSystem, logger)
{
    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.Project;

    private static readonly char[] _filePathSeparator = { '\\', '/' };

    private readonly IReadOnlyCollection<string> _manifests =
    [
        "deployment.yaml",
        "service.yaml",
    ];

    private readonly IReadOnlyCollection<int> _containerPorts =
    [
        8080,
    ];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<AspireProject>(ref reader);

    public override async Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        LogHandlerExecution(logger, nameof(ProjectProcessor), resourceOutputPath);

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var project = resource.Value as AspireProject;

        var containerDetails = await GetContainerDetails(resource.Key, project);

        var data = new ProjectTemplateData(
            resource.Key,
            containerDetails.GetFullImage(),
            project.Env,
            _containerPorts,
            _manifests);

        CreateDeployment(resourceOutputPath, data);
        CreateService(resourceOutputPath, data);
        CreateComponentKustomizeManifest(resourceOutputPath, data);

        return true;
    }

    private async Task<ContainerDetails> GetContainerDetails(string resourceName, AspireProject project)
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
            containerProperties.Properties.ContainerImage ?? GetDefaultImageName(project),
            containerProperties.Properties.ContainerImageTag);
    }

    private static string GetDefaultImageName(AspireProject project)
    {
        var pathSpan = project.Path.AsSpan();
        int lastSeparatorIndex = pathSpan.LastIndexOfAny(_filePathSeparator);
        int dotIndex = pathSpan.LastIndexOf('.');
        var fileNameSpan = pathSpan.Slice(lastSeparatorIndex + 1, dotIndex - lastSeparatorIndex - 1);

        return fileNameSpan.ToString().Kebaberize();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Creating manifest in handler {Handler} at output path: {OutputPath}")]
    static partial void LogHandlerExecution(ILogger logger, string handler, string outputPath);
}


