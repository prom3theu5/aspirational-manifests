using AspireProject = Aspirate.Contracts.Models.AspireManifests.Components.V0.Project;

namespace Aspirate.Cli.Processors.Components.Project;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class ProjectProcessor(
    IFileSystem fileSystem,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService)
        : BaseProcessor<ProjectTemplateData>(fileSystem)
{
    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.Project;

    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.DeploymentType}.yml",
        $"{TemplateLiterals.ServiceType}.yml",
    ];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<AspireProject>(ref reader);

    public override async Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var project = resource.Value as AspireProject;

        var containerDetails = await containerDetailsService.GetContainerDetails(resource.Key, project);
        var containerImage = containerDetailsService.GetFullImage(containerDetails, project);

        ArgumentNullException.ThrowIfNull(containerDetails, nameof(containerDetails));

        var data = new ProjectTemplateData(
            resource.Key,
            containerImage,
            project.Env,
            _manifests);

        CreateDeployment(resourceOutputPath, data);
        CreateService(resourceOutputPath, data);
        CreateComponentKustomizeManifest(resourceOutputPath, data);

        LogCompletion(resourceOutputPath);

        return true;
    }

    public async Task<bool> BuildAndPushProjectContainer(KeyValuePair<string, Resource> resource)
    {
        var project = resource.Value as AspireProject;

        var result = await containerCompositionService.BuildAndPushContainerForProject(project);

        AnsiConsole.MarkupLine($"\t[green](✔) Done: [/] Building and Pushing container for project [blue]{resource.Key}[/]");

        return result;
    }
}


