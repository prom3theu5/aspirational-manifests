using Aspirate.DockerCompose.Models;
using Volume = Aspirate.Shared.Models.AspireManifests.Components.V0.Volume;

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
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
        : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
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
        bool? withPrivateRegistry = false)
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

        KuberizeVolumeNames(container.Volumes);

        var containerPorts = container.Bindings?.Select(b => new Ports { Name = b.Key, Port = b.Value.TargetPort.GetValueOrDefault() }).ToList() ?? [];

        var data = new KubernetesDeploymentTemplateData()
            .SetName(resource.Key)
            .SetContainerImage(container.Image)
            .SetImagePullPolicy(imagePullPolicy)
            .SetEnv(GetFilteredEnvironmentalVariables(resource.Value, disableSecrets))
            .SetAnnotations(container.Annotations)
            .SetVolumes(container.Volumes)
            .SetSecrets(GetSecretEnvironmentalVariables(resource.Value, disableSecrets))
            .SetSecretsFromSecretState(resource, secretProvider, disableSecrets)
            .SetPorts(containerPorts)
            .SetArgs(container.Args)
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

    private static void KuberizeVolumeNames(List<Volume> containerVolumes)
    {
        if (containerVolumes.Count == 0)
        {
            return;
        }

        foreach (var volume in containerVolumes)
        {
            volume.Name = volume.Name.Replace("/", "-").Replace(".", "-").ToLowerInvariant();
        }
    }

    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource)
    {
        var response = new ComposeService();

        var container = resource.Value as ContainerResource;

        var containerPorts = container.Bindings?.Select(b => new Ports { Name = b.Key, Port = b.Value.TargetPort.GetValueOrDefault() }).ToList() ?? [];

        var environment = new Dictionary<string, string?>();

        if (resource.Value is IResourceWithEnvironmentalVariables { Env: not null } resourceWithEnv)
        {
            foreach (var entry in resourceWithEnv.Env.Where(entry => !string.IsNullOrEmpty(entry.Value)))
            {
                environment.Add(entry.Key, entry.Value);
            }
        }

        var service = Builder.MakeService(resource.Key)
            .WithImage(container.Image.ToLowerInvariant());

        if (container.Args is not null)
        {
            service.WithCommands(container.Args.ToArray());
        }

        var composeVolumes = new List<string>();

        if (container.Volumes.Count > 0)
        {
            KuberizeVolumeNames(container.Volumes);

            composeVolumes.AddRange(container.Volumes.Where(x=>!string.IsNullOrWhiteSpace(x.Name)).Select(volume => $"{volume.Name}:{volume.Target}"));
        }

        response.Service = service
            .WithEnvironment(environment)
            .WithContainerName(resource.Key)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .WithVolumes(composeVolumes.ToArray())
            .WithPortMappings(containerPorts.Select(x=> new Port
            {
                Target = x.Port,
                Published = x.Port,
            }).ToArray())
            .Build();

        return response;
    }
}


