namespace Aspirate.Processors.Resources.AbstractProcessors;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class ContainerProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService,
    IManifestWriter manifestWriter)
        : BaseResourceProcessor(fileSystem, console, manifestWriter)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.Container;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<ContainerResource>(ref reader);

    public override Task<bool> CreateManifests(CreateManifestsOptions options)
    {
        var resourceOutputPath = Path.Combine(options.OutputPath, options.Resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var container = options.Resource.Value as ContainerResource;

        var manifests = new List<string>
        {
            container.Volumes.Count > 0
                ? $"{TemplateLiterals.StatefulSetType}.yaml"
                : $"{TemplateLiterals.DeploymentType}.yaml",
            $"{TemplateLiterals.ServiceType}.yaml",
        };

        var data = PopulateKubernetesDeploymentData(options, container, manifests);

        if (container.Volumes.Count > 0)
        {
            _manifestWriter.CreateStatefulSet(resourceOutputPath, data, options.TemplatePath);
        }
        else
        {
            _manifestWriter.CreateDeployment(resourceOutputPath, data, options.TemplatePath);
        }

        _manifestWriter.CreateService(resourceOutputPath, data, options.TemplatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, options.TemplatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    private KubernetesDeploymentData PopulateKubernetesDeploymentData(BaseKubernetesCreateOptions options, ContainerResource? container, List<string> manifests) =>
        new KubernetesDeploymentData()
            .SetWithDashboard(options.WithDashboard.GetValueOrDefault())
            .SetName(options.Resource.Key)
            .SetContainerImage(container.Image)
            .SetImagePullPolicy(options.ImagePullPolicy)
            .SetEnv(GetFilteredEnvironmentalVariables(options.Resource.Value, options.DisableSecrets))
            .SetAnnotations(container.Annotations)
            .SetVolumes(container.Volumes.KuberizeVolumeNames(options.Resource))
            .SetSecrets(GetSecretEnvironmentalVariables(options.Resource.Value, options.DisableSecrets))
            .SetSecretsFromSecretState(options.Resource, secretProvider, options.DisableSecrets)
            .SetPorts(options.Resource.MapBindingsToPorts())
            .SetArgs(container.Args)
            .SetEntrypoint(container.Entrypoint)
            .SetManifests(manifests)
            .SetWithPrivateRegistry(options.WithPrivateRegistry.GetValueOrDefault())
            .Validate();

    public override ComposeService CreateComposeEntry(CreateComposeEntryOptions options)
    {
        var response = new ComposeService();

        var container = options.Resource.Value as ContainerResource;

        var service = Builder.MakeService(options.Resource.Key)
            .WithImage(container.Image.ToLowerInvariant());

        if (container.Args is not null)
        {
            service.WithCommands(container.Args.ToArray());
        }

        var newService = service
            .WithEnvironment(options.Resource.MapResourceToEnvVars(options.WithDashboard))
            .WithContainerName(options.Resource.Key);

        if (!string.IsNullOrEmpty(container.Entrypoint))
        {
            newService = newService.WithCommands(container.Entrypoint);
        }

        response.Service = newService.WithRestartPolicy(ERestartMode.UnlessStopped)
            .WithVolumes(options.Resource.MapComposeVolumes())
            .WithPortMappings(options.Resource.MapBindingsToPorts().MapPortsToDockerComposePorts())
            .Build();

        return response;
    }

    public override List<object> CreateKubernetesObjects(CreateKubernetesObjectsOptions options)
    {
        var container = options.Resource.Value as ContainerResource;
        var data = PopulateKubernetesDeploymentData(options, container, []);

        var objects = new List<object>();

        if (data.Env is not null)
        {
            objects.Add(data.ToKubernetesConfigMap());
        }

        if (data.Secrets is not null)
        {
            objects.Add(data.ToKubernetesSecret());
        }

        switch (data.HasVolumes)
        {
            case true:
                objects.Add(data.ToKubernetesStatefulSet());
                break;
            case false:
                objects.Add(data.ToKubernetesDeployment());
                break;
        }

        objects.Add(data.ToKubernetesService());

        return objects;
    }
}
