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

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource,
        string outputPath,
        string imagePullPolicy,
        string? templatePath = null,
        bool? disableSecrets = false,
        bool? withPrivateRegistry = false,
        bool? withDashboard = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var container = resource.Value as ContainerResource;

        var manifests = new List<string>
        {
            container.Volumes.Count > 0
                ? $"{TemplateLiterals.StatefulSetType}.yml"
                : $"{TemplateLiterals.DeploymentType}.yml",
            $"{TemplateLiterals.ServiceType}.yml",
        };

        var data = new KubernetesDeploymentTemplateData()
            .SetWithDashboard(withDashboard.GetValueOrDefault())
            .SetName(resource.Key)
            .SetContainerImage(container.Image)
            .SetImagePullPolicy(imagePullPolicy)
            .SetEnv(GetFilteredEnvironmentalVariables(resource.Value, disableSecrets))
            .SetAnnotations(container.Annotations)
            .SetVolumes(container.Volumes.KuberizeVolumeNames(resource))
            .SetSecrets(GetSecretEnvironmentalVariables(resource.Value, disableSecrets))
            .SetSecretsFromSecretState(resource, secretProvider, disableSecrets)
            .SetPorts(resource.MapBindingsToPorts())
            .SetArgs(container.Args)
            .SetEntrypoint(container.Entrypoint)
            .SetManifests(manifests)
            .SetWithPrivateRegistry(withPrivateRegistry.GetValueOrDefault())
            .Validate();

        if (container.Volumes.Count > 0)
        {
            _manifestWriter.CreateStatefulSet(resourceOutputPath, data, templatePath);
        }
        else
        {
            _manifestWriter.CreateDeployment(resourceOutputPath, data, templatePath);
        }

        _manifestWriter.CreateService(resourceOutputPath, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }



    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource, bool? withDashboard = false)
    {
        var response = new ComposeService();

        var container = resource.Value as ContainerResource;

        var service = Builder.MakeService(resource.Key)
            .WithImage(container.Image.ToLowerInvariant());

        if (container.Args is not null)
        {
            service.WithCommands(container.Args.ToArray());
        }

        var newService = service
            .WithEnvironment(resource.MapResourceToEnvVars(withDashboard))
            .WithContainerName(resource.Key);

        if (!string.IsNullOrEmpty(container.Entrypoint))
        {
            newService = newService.WithCommands(container.Entrypoint);
        }

        response.Service = newService.WithRestartPolicy(RestartMode.UnlessStopped)
            .WithVolumes(resource.MapComposeVolumes())
            .WithPortMappings(resource.MapBindingsToPorts().MapPortsToDockerComposePorts())
            .Build();

        return response;
    }
}


