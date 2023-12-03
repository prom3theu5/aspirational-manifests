using Aspirate.Shared.Models.AspireManifests.Components.V0;
using AspireDockerfile = Aspirate.Shared.Models.AspireManifests.Components.V0.Dockerfile;

namespace Aspirate.Processors.Dockerfile;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class DockerfileProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
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

    private readonly Dictionary<string, string> _containerImageCache = [];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<AspireDockerfile>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string? templatePath = null)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var dockerFile = resource.Value as AspireDockerfile;

        var containerPorts = dockerFile.Bindings?.Select(b => new Ports { Name = b.Key, Port = int.Parse(b.Value.ContainerPort) }).ToList() ?? [];

        if (!_containerImageCache.TryGetValue(resource.Key, out var containerImage))
        {
            throw new InvalidOperationException($"Container Image for dockerfile {resource.Key} not found.");
        }

        var data = new DockerfileTemplateData(
            resource.Key,
            containerImage,
            dockerFile.Env,
            containerPorts,
            _manifests);

        CreateDeployment(resourceOutputPath, data, templatePath);
        CreateService(resourceOutputPath, data, templatePath);
        CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public async Task BuildAndPushContainerForDockerfile(KeyValuePair<string, Resource> resource, string builder, string imageName, string registry, bool nonInteractive)
    {
        var dockerfile = resource.Value as AspireDockerfile;

        await containerCompositionService.BuildAndPushContainerForDockerfile(dockerfile, builder, imageName, registry, nonInteractive);

        _containerImageCache.Add(resource.Key, $"{registry}/{imageName}:latest");

        _console.MarkupLine($"\t[green]({EmojiLiterals.CheckMark}) Done: [/] Building and Pushing container for Dockerfile [blue]{resource.Key}[/]");
    }
}


