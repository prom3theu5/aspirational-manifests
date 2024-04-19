namespace Aspirate.Processors.Resources.Dockerfile;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class DockerfileProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService,
    IManifestWriter manifestWriter)
    : BaseResourceProcessor(fileSystem, console, manifestWriter)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.Dockerfile;

    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.DeploymentType}.yml",
        $"{TemplateLiterals.ServiceType}.yml",
    ];

    private readonly Dictionary<string, string> _containerImageCache = [];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<DockerfileResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false,
        bool? withPrivateRegistry = false,
        bool? withDashboard = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var dockerFile = resource.Value as DockerfileResource;

        if (!_containerImageCache.TryGetValue(resource.Key, out var containerImage))
        {
            throw new InvalidOperationException($"Container Image for dockerfile {resource.Key} not found.");
        }

        var data = new KubernetesDeploymentTemplateData()
            .SetWithDashboard(withDashboard.GetValueOrDefault())
            .SetName(resource.Key)
            .SetContainerImage(containerImage)
            .SetImagePullPolicy(imagePullPolicy)
            .SetEnv(GetFilteredEnvironmentalVariables(resource.Value, disableSecrets))
            .SetAnnotations(dockerFile.Annotations)
            .SetSecrets(GetSecretEnvironmentalVariables(resource.Value, disableSecrets))
            .SetSecretsFromSecretState(resource, secretProvider, disableSecrets)
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

    public async Task BuildAndPushContainerForDockerfile(KeyValuePair<string, Resource> resource, ContainerParameters parameters, bool nonInteractive)
    {
        var dockerfile = resource.Value as DockerfileResource;

        await containerCompositionService.BuildAndPushContainerForDockerfile(dockerfile, parameters, nonInteractive);

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Building and Pushing container for Dockerfile [blue]{resource.Key}[/]");
    }

    public void PopulateContainerImageCacheWithImage(KeyValuePair<string, Resource> resource, ContainerParameters parameters)
    {
        _containerImageCache.Add(resource.Key, parameters.ToImageName(resource.Key));

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Setting container details for Dockerfile [blue]{resource.Key}[/]");
    }

    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource, bool? withDashboard = false)
    {
        var response = new ComposeService();

        if (!_containerImageCache.TryGetValue(resource.Key, out var containerImage))
        {
            throw new InvalidOperationException($"Container Image for dockerfile {resource.Key} not found.");
        }

        response.Service = Builder.MakeService(resource.Key)
            .WithImage(containerImage.ToLowerInvariant())
            .WithEnvironment(resource.MapResourceToEnvVars(withDashboard))
            .WithContainerName(resource.Key)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .WithPortMappings(resource.MapBindingsToPorts().MapPortsToDockerComposePorts())
            .Build();

        return response;
    }


}


