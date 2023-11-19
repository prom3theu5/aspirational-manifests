namespace Aspirational.Manifests.ManifestHandlers.Components.Project;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class ProjectHandler : BaseHandler<ProjectTemplateData>
{
    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.Project;

    private readonly IReadOnlyCollection<string> _manifests =
    [
        "deployment.yaml",
        "service.yaml",
    ];

    private readonly IReadOnlyCollection<int> _containerPorts =
    [
        8080,
    ];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<Models.Components.V0.Project>(ref reader);

    public override bool CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);
        AnsiConsole.MarkupLine($"[green]Creating manifest in handler {GetType().Name} at output path: {resourceOutputPath}[/]");

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var project = resource.Value as Models.Components.V0.Project;

        var data = new ProjectTemplateData(
            resource.Key,
            project.Env,
            _containerPorts,
            _manifests,
            true);

        CreateDeployment(resourceOutputPath, data);
        CreateService(resourceOutputPath, data);
        CreateComponentKustomizeManifest(resourceOutputPath, data);

        return true;
    }
}


