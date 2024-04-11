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
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.Dockerfile;

    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.DeploymentType}.yml",
        $"{TemplateLiterals.ServiceType}.yml",
    ];

    private readonly StringBuilder _tagBuilder = new();

    private readonly Dictionary<string, string> _containerImageCache = [];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<DockerfileResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false,
        bool? withPrivateRegistry = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var dockerFile = resource.Value as DockerfileResource;

        var containerPorts = dockerFile.Bindings?.Select(b => new Ports { Name = b.Key, Port = b.Value.TargetPort.GetValueOrDefault() }).ToList() ?? [];

        if (!_containerImageCache.TryGetValue(resource.Key, out var containerImage))
        {
            throw new InvalidOperationException($"Container Image for dockerfile {resource.Key} not found.");
        }

        var data = new KubernetesDeploymentTemplateData()
            .SetName(resource.Key)
            .SetContainerImage(containerImage)
            .SetImagePullPolicy(imagePullPolicy)
            .SetEnv(GetFilteredEnvironmentalVariables(resource.Value, disableSecrets))
            .SetAnnotations(dockerFile.Annotations)
            .SetSecrets(GetSecretEnvironmentalVariables(resource.Value, disableSecrets))
            .SetSecretsFromSecretState(resource, secretProvider, disableSecrets)
            .SetPorts(containerPorts)
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
        _containerImageCache.Add(resource.Key, _tagBuilder.ToString());

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Setting container details for Dockerfile [blue]{resource.Key}[/]");
    }

    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource)
    {
        var response = new ComposeService();

        var dockerFile = resource.Value as DockerfileResource;

        var containerPorts = dockerFile.Bindings?.Select(b => new Ports { Name = b.Key, Port = b.Value.TargetPort.GetValueOrDefault() }).ToList() ?? [];

        if (!_containerImageCache.TryGetValue(resource.Key, out var containerImage))
        {
            throw new InvalidOperationException($"Container Image for dockerfile {resource.Key} not found.");
        }

        var environment = new Dictionary<string, string?>();

        if (resource.Value is IResourceWithEnvironmentalVariables { Env: not null } resourceWithEnv)
        {
            foreach (var entry in resourceWithEnv.Env)
            {
                environment.Add(entry.Key, entry.Value);
            }
        }

        response.Service = Builder.MakeService(resource.Key)
            .WithImage(containerImage.ToLowerInvariant())
            .WithEnvironment(environment)
            .WithContainerName(resource.Key)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .WithPortMappings(containerPorts.Select(x=> new Port
            {
                Target = x.Port,
                Published = x.Port,
            }).ToArray())
            .Build();

        return response;
    }


}


