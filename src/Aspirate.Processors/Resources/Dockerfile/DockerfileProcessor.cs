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
        $"{TemplateLiterals.DeploymentType}.yaml",
        $"{TemplateLiterals.ServiceType}.yaml",
    ];

    private readonly Dictionary<string, string> _containerImageCache = [];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<DockerfileResource>(ref reader);

    public override Task<bool> CreateManifests(CreateManifestsOptions options)
    {
        var resourceOutputPath = Path.Combine(options.OutputPath, options.Resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var dockerFile = options.Resource.Value as DockerfileResource;

        if (!_containerImageCache.TryGetValue(options.Resource.Key, out var containerImage))
        {
            throw new InvalidOperationException($"Container Image for dockerfile {options.Resource.Key} not found.");
        }

        var data = new KubernetesDeploymentTemplateData()
            .SetWithDashboard(options.WithDashboard.GetValueOrDefault())
            .SetName(options.Resource.Key)
            .SetContainerImage(containerImage)
            .SetImagePullPolicy(options.ImagePullPolicy)
            .SetArgs(dockerFile.Args)
            .SetEnv(GetFilteredEnvironmentalVariables(options.Resource.Value, options.DisableSecrets))
            .SetAnnotations(dockerFile.Annotations)
            .SetSecrets(GetSecretEnvironmentalVariables(options.Resource.Value, options.DisableSecrets))
            .SetSecretsFromSecretState(options.Resource, secretProvider, options.DisableSecrets)
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

    public async Task BuildAndPushContainerForDockerfile(KeyValuePair<string, Resource> resource, ContainerOptions options, bool nonInteractive)
    {
        var dockerfile = resource.Value as DockerfileResource;

        await containerCompositionService.BuildAndPushContainerForDockerfile(dockerfile, options, nonInteractive);

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Building and Pushing container for Dockerfile [blue]{resource.Key}[/]");
    }

    public void PopulateContainerImageCacheWithImage(KeyValuePair<string, Resource> resource, ContainerOptions options)
    {
        _containerImageCache.Add(resource.Key, options.ToImageName(resource.Key));

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Setting container details for Dockerfile [blue]{resource.Key}[/]");
    }

    public override ComposeService CreateComposeEntry(CreateComposeEntryOptions options)
    {
        var response = new ComposeService();

        var dockerfile = options.Resource.Value as DockerfileResource;

        var newService = Builder.MakeService(options.Resource.Key)
            .WithEnvironment(options.Resource.MapResourceToEnvVars(options.WithDashboard))
            .WithContainerName(options.Resource.Key)
            .WithRestartPolicy(ERestartMode.UnlessStopped)
            .WithPortMappings(options.Resource.MapBindingsToPorts().MapPortsToDockerComposePorts());

        if (options.ComposeBuilds == true)
        {
            newService = newService.WithBuild(builder =>
            {
                builder.WithContext(dockerfile.Context)
                    .WithDockerfile(_fileSystem.GetFullPath(dockerfile.Path))
                    .Build();
            });
        }
        else
        {
            if (!_containerImageCache.TryGetValue(options.Resource.Key, out var containerImage))
            {
                throw new InvalidOperationException($"Container Image for dockerfile {options.Resource.Key} not found.");
            }

            newService = newService.WithImage(containerImage.ToLowerInvariant());
        }

        response.Service = newService.Build();

        return response;
    }
}
