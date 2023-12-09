using AspireContainer = Aspirate.Shared.Models.AspireManifests.Components.V0.Container;

namespace Aspirate.Processors.Container;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class ContainerProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService)
        : BaseProcessor<ContainerTemplateData>(fileSystem, console)
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
        JsonSerializer.Deserialize<AspireContainer>(ref reader);

    /// <inheritdoc />
    public override void ReplacePlaceholders(Resource resource, Dictionary<string, Resource> resources)
    {
        base.ReplacePlaceholders(resource, resources);

        if (resource is not AspireContainer {ConnectionString: not null} container)
        {
            return;
        }

        container.ConnectionString = ReplaceConnectionStringPlaceholders(container, resources);
    }

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var container = resource.Value as AspireContainer;

        var containerPorts = container.Bindings?.Select(b => new Ports { Name = b.Key, Port = b.Value.ContainerPort }).ToList() ?? [];


        var data = disableSecrets is false
            ? HandleWithSecrets(resource, container, containerPorts)
            : HandleDisabledSecrets(resource, container, containerPorts);

        CreateDeployment(resourceOutputPath, data, templatePath);
        CreateService(resourceOutputPath, data, templatePath);
        CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    private ContainerTemplateData HandleWithSecrets(KeyValuePair<string, Resource> resource, AspireContainer container, List<Ports> containerPorts)
    {
        var envVars = GetFilteredEnvironmentalVariables(resource.Value);
        var secrets = GetSecretEnvironmentalVariables(resource.Value);

        SetSecretsFromSecretState(secrets, resource, secretProvider);

        return new(
            resource.Key,
            container.Image,
            envVars,
            secrets,
            containerPorts,
            _manifests);
    }

    private ContainerTemplateData HandleDisabledSecrets(KeyValuePair<string, Resource> resource, AspireContainer container, List<Ports> containerPorts) =>
        new(resource.Key,
            container.Image,
            resource.Value.Env,
            null,
            containerPorts,
            _manifests);
}


