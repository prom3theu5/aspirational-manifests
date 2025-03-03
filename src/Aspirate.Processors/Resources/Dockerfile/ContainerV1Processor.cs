namespace Aspirate.Processors.Resources.Dockerfile;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class ContainerV1Processor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService,
    IManifestWriter manifestWriter)
    : BaseResourceProcessor(fileSystem, console, manifestWriter), IDockerBuildProcessor
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.ContainerV1;

    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.DeploymentType}.yaml",
        $"{TemplateLiterals.ServiceType}.yaml",
    ];

    private readonly Dictionary<string, List<string>> _containerImageCache = [];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<ContainerResourceV1>(ref reader);

    public override Task<bool> CreateManifests(CreateManifestsOptions options)
    {
        var resourceOutputPath = Path.Combine(options.OutputPath, options.Resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var cv1 = options.Resource.Value as ContainerResourceV1;

        if (!_containerImageCache.TryGetValue(options.Resource.Key, out var containerImages))
        {
            throw new InvalidOperationException($"Container Image for dockerfile {options.Resource.Key} not found.");
        }

        var data = PopulateKubernetesDeploymentData(options, containerImages.First(), cv1);

        _manifestWriter.CreateDeployment(resourceOutputPath, data, options.TemplatePath);
        _manifestWriter.CreateService(resourceOutputPath, data, options.TemplatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, options.TemplatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    private KubernetesDeploymentData PopulateKubernetesDeploymentData(BaseKubernetesCreateOptions options, string containerImage, ContainerResourceV1? cv1) =>
        new KubernetesDeploymentData()
            .SetWithDashboard(options.WithDashboard.GetValueOrDefault())
            .SetName(options.Resource.Key)
            .SetContainerImage(containerImage)
            .SetImagePullPolicy(options.ImagePullPolicy)
            //.SetArgs(dockerFile.Args)
            .SetEnv(GetFilteredEnvironmentalVariables(options.Resource, options.DisableSecrets, options.WithDashboard))
            //.SetAnnotations(dockerFile.Annotations)
            .SetSecrets(GetSecretEnvironmentalVariables(options.Resource, options.DisableSecrets, options.WithDashboard))
            .SetSecretsFromSecretState(options.Resource, secretProvider, options.DisableSecrets)
            .SetPorts(options.Resource.MapBindingsToPorts())
            .SetManifests(_manifests)
            .SetWithPrivateRegistry(options.WithPrivateRegistry.GetValueOrDefault())
            .Validate();

    public async Task BuildAndPushContainerForDockerfile(KeyValuePair<string, Resource> resource, ContainerOptions options, bool nonInteractive)
    {
        var cv1 = resource.Value as ContainerResourceV1;
        if (cv1 == null)
            return;

        DockerfileResource dockerfile = new()
        {
            Path = cv1.Build.Dockerfile,
            Context = cv1.Build.Context,
            Bindings = cv1.Bindings,
            Annotations = cv1.Annotations,
            Env = cv1.Env,
            //BuildArgs = cv1.BuildArgs,
            Args = cv1.Args
        };

        await containerCompositionService.BuildAndPushContainerForDockerfile(dockerfile, options, nonInteractive);

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Building and Pushing container for Dockerfile [blue]{resource.Key}[/]");
    }

    public void PopulateContainerImageCacheWithImage(KeyValuePair<string, Resource> resource, ContainerOptions options)
    {
        var cv1 = resource.Value as ContainerResourceV1;
        if (cv1 == null)
            return;

        _containerImageCache.Add(resource.Key, options.ToImageNames(resource.Key));

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Setting container details for Dockerfile [blue]{resource.Key}[/]");
    }

    public override ComposeService CreateComposeEntry(CreateComposeEntryOptions options)
    {
        var response = new ComposeService();

        var cv1 = options.Resource.Value as ContainerResourceV1;

        var newService = Builder.MakeService(options.Resource.Key)
            .WithEnvironment(options.Resource.MapResourceToEnvVars(options.WithDashboard))
            .WithContainerName(options.Resource.Key)
            .WithRestartPolicy(ERestartMode.UnlessStopped)
            .WithPortMappings(options.Resource.MapBindingsToPorts().MapPortsToDockerComposePorts());

        if (options.ComposeBuilds == true)
        {
            newService = newService.WithBuild(builder =>
            {
                builder.WithContext(cv1.Build.Context)
                    .WithDockerfile(_fileSystem.GetFullPath(cv1.Build.Dockerfile))
                    .Build();
            });
        }
        else
        {
            if (!_containerImageCache.TryGetValue(options.Resource.Key, out var containerImage))
            {
                throw new InvalidOperationException($"Container Image for dockerfile {options.Resource.Key} not found.");
            }

            newService = newService.WithImage(containerImage[0].ToLowerInvariant());
        }

        response.Service = newService.Build();

        return response;
    }

    public override List<object> CreateKubernetesObjects(CreateKubernetesObjectsOptions options)
    {
        var cv1 = options.Resource.Value as ContainerResourceV1;

        if (!_containerImageCache.TryGetValue(options.Resource.Key, out var containerImage))
        {
            throw new InvalidOperationException($"Container Image for dockerfile {options.Resource.Key} not found.");
        }

        var data = PopulateKubernetesDeploymentData(options, containerImage[0], cv1);

        return data.ToKubernetesObjects(options.EncodeSecrets);
    }
}
