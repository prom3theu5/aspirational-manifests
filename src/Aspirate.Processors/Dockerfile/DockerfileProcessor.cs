using AspireDockerfile = Aspirate.Shared.Models.AspireManifests.Components.V0.Dockerfile;

namespace Aspirate.Processors.Dockerfile;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class DockerfileProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService)
        : BaseProcessor<DockerfileTemplateData>(fileSystem, console)
{
    /// <inheritdoc />
    public override string ResourceType => AspireLiterals.Dockerfile;

    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.DeploymentType}.yml",
        $"{TemplateLiterals.ServiceType}.yml",
    ];

    private readonly StringBuilder _tagBuilder = new();

    private readonly Dictionary<string, string> _containerImageCache = [];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<AspireDockerfile>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var dockerFile = resource.Value as AspireDockerfile;

        var containerPorts = dockerFile.Bindings?.Select(b => new Ports { Name = b.Key, Port = b.Value.ContainerPort }).ToList() ?? [];

        if (!_containerImageCache.TryGetValue(resource.Key, out var containerImage))
        {
            throw new InvalidOperationException($"Container Image for dockerfile {resource.Key} not found.");
        }

        var data = disableSecrets is false
            ? HandleWithSecrets(resource, containerImage, containerPorts, imagePullPolicy)
            : HandleDisabledSecrets(resource, containerImage, containerPorts, imagePullPolicy);

        CreateDeployment(resourceOutputPath, data, templatePath);
        CreateService(resourceOutputPath, data, templatePath);
        CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    private DockerfileTemplateData HandleWithSecrets(KeyValuePair<string, Resource> resource, string containerImage,
        List<Ports> containerPorts, string imagePullPolicy)
    {
        var envVars = GetFilteredEnvironmentalVariables(resource.Value);
        var secrets = GetSecretEnvironmentalVariables(resource.Value);

        SetSecretsFromSecretState(secrets, resource, secretProvider);

        return new(
            resource.Key,
            containerImage,
            envVars,
            secrets,
            containerPorts,
            _manifests,
            imagePullPolicy);
    }

    private DockerfileTemplateData HandleDisabledSecrets(KeyValuePair<string, Resource> resource, string containerImage,
        List<Ports> containerPorts, string imagePullPolicy) =>
        new(resource.Key,
            containerImage,
            resource.Value.Env,
            null,
            containerPorts,
            _manifests,
            imagePullPolicy);

    public async Task BuildAndPushContainerForDockerfile(KeyValuePair<string, Resource> resource, string builder, string imageName, string registry, bool nonInteractive)
    {
        _tagBuilder.Clear();

        if (!string.IsNullOrEmpty(registry))
        {
            _tagBuilder.Append($"{registry}/");
        }

        _tagBuilder.Append(imageName);
        _tagBuilder.Append(":latest");

        var dockerfile = resource.Value as AspireDockerfile;

        await containerCompositionService.BuildAndPushContainerForDockerfile(dockerfile, builder, imageName, registry, nonInteractive);

        _containerImageCache.Add(resource.Key, _tagBuilder.ToString());

        _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Building and Pushing container for Dockerfile [blue]{resource.Key}[/]");
    }
}


