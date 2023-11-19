namespace Aspirational.Manifests.ManifestHandlers.Components;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class ProjectHandler : BaseHandler
{
    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.Project;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<Project>(ref reader);

    public override bool CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);
        AnsiConsole.MarkupLine($"[green]Creating manifest in handler {GetType().Name} at output path: {resourceOutputPath}[/]");

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var project = resource.Value as Project;

        var manifests = new List<string>
        {
            "deployment.yaml",
            "service.yaml",
        };

        var data = new
        {
            Name = resource.Key,
            project.Env,
            project.Path,
            project.Bindings,
            Manifests = manifests,
            IsService = true,
        };

        CreateDeployment(resourceOutputPath, data);
        CreateService(resourceOutputPath, data);
        CreateComponentKustomizeManifest(resourceOutputPath, data);

        return true;
    }
}
