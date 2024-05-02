using Aspirate.Shared.Inputs;
using Aspirate.Shared.Interfaces.Secrets;
using Aspirate.Shared.Interfaces.Services;

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

    public override Task<bool> CreateManifests(CreateManifestsOptions options)
    {
        var resourceOutputPath = Path.Combine(options.OutputPath, options.Resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        if (!_containerDetailsCache.TryGetValue(options.Resource.Key, out var containerDetails))
        {
            throw new InvalidOperationException($"Container details for project {options.Resource.Key} not found.");
        }

        var project = options.Resource.Value as ProjectResource;

        var data = new KubernetesDeploymentTemplateData()
            .SetWithDashboard(options.WithDashboard.GetValueOrDefault())
            .SetName(options.Resource.Key)
            .SetContainerImage(containerDetails.FullContainerImage)
            .SetImagePullPolicy(options.ImagePullPolicy)
            .SetEnv(GetFilteredEnvironmentalVariables(options.Resource.Value, options.DisableSecrets))
            .SetAnnotations(project.Annotations)
            .SetArgs(project.Args)
            .SetSecrets(GetSecretEnvironmentalVariables(options.Resource.Value, options.DisableSecrets))
            .SetSecretsFromSecretState(options.Resource, secretProvider, options.DisableSecrets)
            .SetIsProject(true)
            .SetPorts(options.Resource.MapBindingsToPorts())
            .SetManifests(_manifests)
            .SetWithPrivateRegistry(options.WithPrivateRegistry.GetValueOrDefault())
            .Validate();

        _manifestWriter.CreateDeployment(resourceOutputPath, data, options.TemplatePath);
        _manifestWriter.CreateService(resourceOutputPath, data, options.TemplatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, options.TemplatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public async Task BuildAndPushProjectContainer(KeyValuePair<string, Resource> resource, ContainerOptions options, bool nonInteractive, string? runtimeIdentifier)
    {
        var project = resource.Value as ProjectResource;

        if (!_containerDetailsCache.TryGetValue(resource.Key, out var containerDetails))
        {
            throw new InvalidOperationException($"Container details for project {resource.Key} not found.");
        }

        await containerCompositionService.BuildAndPushContainerForProject(project, containerDetails, options, nonInteractive, runtimeIdentifier);

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Building and Pushing container for project [blue]{resource.Key}[/]");
    }

    public async Task PopulateContainerDetailsCacheForProject(KeyValuePair<string, Resource> resource, ContainerOptions options)
    {
        var project = resource.Value as ProjectResource;

        var details = await containerDetailsService.GetContainerDetails(resource.Key, project, options);

        var success = _containerDetailsCache.TryAdd(resource.Key, details);

        if (!success)
        {
            throw new InvalidOperationException($"Failed to add container details for project {resource.Key} to cache.");
        }

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Populated container details cache for project [blue]{resource.Key}[/]");
    }

    public override ComposeService CreateComposeEntry(CreateComposeEntryOptions options)
    {
        var response = new ComposeService();

        if (!_containerDetailsCache.TryGetValue(options.Resource.Key, out var containerDetails))
        {
            throw new InvalidOperationException($"Container details for project {options.Resource.Key} not found.");
        }

        response.Service = Builder.MakeService(options.Resource.Key)
            .WithEnvironment(options.Resource.MapResourceToEnvVars(options.WithDashboard))
            .WithContainerName(options.Resource.Key)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .WithPortMappings(options.Resource.MapBindingsToPorts().MapPortsToDockerComposePorts())
            .WithImage(containerDetails.FullContainerImage.ToLowerInvariant())
            .Build();

        response.IsProject = true;

        return response;
    }
}


