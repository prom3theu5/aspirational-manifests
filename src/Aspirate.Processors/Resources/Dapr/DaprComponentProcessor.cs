namespace Aspirate.Processors.Resources.Dapr;

public class DaprComponentProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    public override string ResourceType => AspireComponentLiterals.DaprComponent;

    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<DaprComponentResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null,
        bool? disableSecrets = false)
    {
        var daprComponentResource = resource.Value as DaprComponentResource;

        if (daprComponentResource?.DaprComponentProperty is null)
        {
            return Task.FromResult(false);
        }

        var templateData = new DaprComponentTemplateData()
            .SetType(daprComponentResource.DaprComponentProperty.Type)
            .SetVersion(daprComponentResource.DaprComponentProperty.Version)
            .SetMetadata(daprComponentResource.DaprComponentProperty.Metadata)
            .SetName(daprComponentResource.Name);

        _manifestWriter.CreateDaprManifest(outputPath, templateData, daprComponentResource.Name, templatePath);

        LogCompletion($"{outputPath}/dapr/{daprComponentResource.Name}.yml");

        return Task.FromResult(true);
    }
}
