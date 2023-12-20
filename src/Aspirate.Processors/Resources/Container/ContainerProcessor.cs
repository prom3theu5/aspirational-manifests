namespace Aspirate.Processors.Resources.Container;

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

    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.DeploymentType}.yml",
        $"{TemplateLiterals.ServiceType}.yml",
    ];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<Shared.Models.AspireManifests.Components.V0.Container>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var container = resource.Value as Shared.Models.AspireManifests.Components.V0.Container;

        var containerPorts = container.Bindings?.Select(b => new Ports { Name = b.Key, Port = b.Value.ContainerPort }).ToList() ?? [];

        var data = new KubernetesDeploymentTemplateData()
            .SetName(resource.Key)
            .SetContainerImage(container.Image)
            .SetEnv(GetFilteredEnvironmentalVariables(resource.Value, disableSecrets))
            .SetSecrets(GetSecretEnvironmentalVariables(resource.Value, disableSecrets))
            .SetSecretsFromSecretState(resource, secretProvider, disableSecrets)
            .SetIsContainer(true)
            .SetPorts(containerPorts)
            .SetManifests(_manifests)
            .Validate();

        _manifestWriter.CreateDeployment(resourceOutputPath, data, templatePath);
        _manifestWriter.CreateService(resourceOutputPath, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public override void ReplacePlaceholders(Resource resource, Dictionary<string, Resource> resources)
    {
        var connectionStringHandler =
            _substitutionStrategies.FirstOrDefault(s => s is ResourceContainerConnectionStringSubstitutionStrategy);

        connectionStringHandler?.Substitute(new KeyValuePair<string, string>(), resources, resource);

        base.ReplacePlaceholders(resource, resources);
    }
}


