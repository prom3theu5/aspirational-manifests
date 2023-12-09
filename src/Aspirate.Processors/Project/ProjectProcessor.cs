namespace Aspirate.Processors.Project;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class ProjectProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService)
        : BaseProcessor<ProjectTemplateData>(fileSystem, console)
{
    /// <inheritdoc />
    public override string ResourceType => AspireLiterals.Project;

    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.DeploymentType}.yml",
        $"{TemplateLiterals.ServiceType}.yml",
    ];

    private readonly Dictionary<string, MsBuildContainerProperties> _containerDetailsCache = [];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<AspireProject>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy, string? templatePath = null)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        if (!_containerDetailsCache.TryGetValue(resource.Key, out var containerDetails))
        {
            throw new InvalidOperationException($"Container details for project {resource.Key} not found.");
        }

        var envVars = GetFilteredEnvironmentalVariables(resource.Value);
        var secrets = GetSecretEnvironmentalVariables(resource.Value);

        SetSecretsFromSecretState(secrets, resource, secretProvider);

        var data = new ProjectTemplateData(
            resource.Key,
            containerDetails.FullContainerImage,
            envVars,
            secrets,
            _manifests,
            imagePullPolicy);

        CreateDeployment(resourceOutputPath, data, templatePath);
        CreateService(resourceOutputPath, data, templatePath);
        CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public async Task BuildAndPushProjectContainer(KeyValuePair<string, Resource> resource, bool nonInteractive)
    {
        var project = resource.Value as AspireProject;

        if (!_containerDetailsCache.TryGetValue(resource.Key, out var containerDetails))
        {
            throw new InvalidOperationException($"Container details for project {resource.Key} not found.");
        }

        await containerCompositionService.BuildAndPushContainerForProject(project, containerDetails, nonInteractive);

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Building and Pushing container for project [blue]{resource.Key}[/]");
    }

    public async Task PopulateContainerDetailsCacheForProject(
        KeyValuePair<string, Resource> resource,
        string containerRegistry,
        string containerImageTag)
    {
        var project = resource.Value as AspireProject;

        var details = await containerDetailsService.GetContainerDetails(resource.Key, project, containerRegistry, containerImageTag);

        var success = _containerDetailsCache.TryAdd(resource.Key, details);

        if (!success)
        {
            throw new InvalidOperationException($"Failed to add container details for project {resource.Key} to cache.");
        }

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Populated container details cache for project [blue]{resource.Key}[/]");
    }
}


