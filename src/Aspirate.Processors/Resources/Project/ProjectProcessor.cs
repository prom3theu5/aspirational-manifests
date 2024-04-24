namespace Aspirate.Processors.Resources.Project;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public sealed class ProjectProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService,
    IManifestWriter manifestWriter)
    : BaseResourceProcessor(fileSystem, console, manifestWriter)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.Project;

    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.DeploymentType}.yml",
        $"{TemplateLiterals.ServiceType}.yml",
    ];

    private readonly Dictionary<string, MsBuildContainerProperties> _containerDetailsCache = [];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<ProjectResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy, string? templatePath = null, bool? disableSecrets = false, bool? withPrivateRegistry = false, bool? withDashboard = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        if (!_containerDetailsCache.TryGetValue(resource.Key, out var containerDetails))
        {
            throw new InvalidOperationException($"Container details for project {resource.Key} not found.");
        }

        var project = resource.Value as ProjectResource;

        var data = new KubernetesDeploymentTemplateData()
            .SetWithDashboard(withDashboard.GetValueOrDefault())
            .SetName(resource.Key)
            .SetContainerImage(containerDetails.FullContainerImage)
            .SetImagePullPolicy(imagePullPolicy)
            .SetEnv(GetFilteredEnvironmentalVariables(resource.Value, disableSecrets))
            .SetAnnotations(project.Annotations)
            .SetArgs(project.Args)
            .SetSecrets(GetSecretEnvironmentalVariables(resource.Value, disableSecrets))
            .SetSecretsFromSecretState(resource, secretProvider, disableSecrets)
            .SetIsProject(true)
            .SetPorts(resource.MapBindingsToPorts())
            .SetManifests(_manifests)
            .SetWithPrivateRegistry(withPrivateRegistry.GetValueOrDefault())
            .Validate();

        _manifestWriter.CreateDeployment(resourceOutputPath, data, templatePath);
        _manifestWriter.CreateService(resourceOutputPath, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public async Task BuildAndPushProjectContainer(KeyValuePair<string, Resource> resource, ContainerParameters parameters, bool nonInteractive, string? runtimeIdentifier)
    {
        var project = resource.Value as ProjectResource;

        if (!_containerDetailsCache.TryGetValue(resource.Key, out var containerDetails))
        {
            throw new InvalidOperationException($"Container details for project {resource.Key} not found.");
        }

        await containerCompositionService.BuildAndPushContainerForProject(project, containerDetails, parameters, nonInteractive, runtimeIdentifier);

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Building and Pushing container for project [blue]{resource.Key}[/]");
    }

    public async Task PopulateContainerDetailsCacheForProject(KeyValuePair<string, Resource> resource, ContainerParameters parameters)
    {
        var project = resource.Value as ProjectResource;

        var details = await containerDetailsService.GetContainerDetails(resource.Key, project, parameters);

        var success = _containerDetailsCache.TryAdd(resource.Key, details);

        if (!success)
        {
            throw new InvalidOperationException($"Failed to add container details for project {resource.Key} to cache.");
        }

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Populated container details cache for project [blue]{resource.Key}[/]");
    }

    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource, bool? withDashboard = false)
    {
        var response = new ComposeService();

        if (!_containerDetailsCache.TryGetValue(resource.Key, out var containerDetails))
        {
            throw new InvalidOperationException($"Container details for project {resource.Key} not found.");
        }

        response.Service = Builder.MakeService(resource.Key)
            .WithImage(containerDetails.FullContainerImage.ToLowerInvariant())
            .WithEnvironment(resource.MapResourceToEnvVars(withDashboard))
            .WithContainerName(resource.Key)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .WithPortMappings(resource.MapBindingsToPorts().MapPortsToDockerComposePorts())
            .Build();

        response.IsProject = true;

        return response;
    }
}


