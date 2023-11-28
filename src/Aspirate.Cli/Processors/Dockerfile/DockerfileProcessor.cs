using AspireDockerfile = Aspirate.Shared.Models.AspireManifests.Components.V0.Dockerfile;

namespace Aspirate.Cli.Processors.Dockerfile;

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

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<AspireDockerfile>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string? templatePath = null)
    {
        // var resourceOutputPath = Path.Combine(outputPath, resource.Key);
        //
        // EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);
        //
        // var project = resource.Value as AspireDockerfile;
        //
        // if (!_containerDetailsCache.TryGetValue(resource.Key, out var containerDetails))
        // {
        //     throw new InvalidOperationException($"Container details for project {resource.Key} not found.");
        // }
        //
        // var data = new DockerfileTemplateData(
        //     resource.Key,
        //     containerDetails.FullContainerImage,
        //     project.Env,
        //     _manifests);
        //
        // CreateDeployment(resourceOutputPath, data, templatePath);
        // CreateService(resourceOutputPath, data, templatePath);
        // CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);
        //
        // LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public async Task BuildAndPushContainerForDockerfile(KeyValuePair<string, Resource> resource, string builder, string imageName, string registry, bool nonInteractive)
    {
        var dockerfile = resource.Value as AspireDockerfile;

        await containerCompositionService.BuildAndPushContainerForDockerfile(dockerfile, builder, imageName, registry, nonInteractive);

        _console.MarkupLine($"\t[green]({EmojiLiterals.CheckMark}) Done: [/] Building and Pushing container for Dockerfile [blue]{resource.Key}[/]");
    }
}


