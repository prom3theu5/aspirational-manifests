namespace Aspirate.Processors.Resources.Dapr;

public class DaprComponentProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    IManifestWriter manifestWriter)
    : BaseResourceProcessor(fileSystem, console, manifestWriter)
{
    public override string ResourceType => AspireComponentLiterals.DaprComponent;

    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<DaprComponentResource>(ref reader);

    public override Task<bool> CreateManifests(CreateManifestsOptions options)
    {
        var daprComponentResource = options.Resource.Value as DaprComponentResource;

        if (daprComponentResource?.DaprComponentProperty is null)
        {
            return Task.FromResult(false);
        }

        var templateData = new DaprComponentTemplateData()
            .SetType(daprComponentResource.DaprComponentProperty.Type)
            .SetVersion(daprComponentResource.DaprComponentProperty.Version)
            .SetMetadata(daprComponentResource.DaprComponentProperty.Metadata)
            .SetName(daprComponentResource.Name);

        _manifestWriter.CreateDaprManifest(options.OutputPath, templateData, daprComponentResource.Name, options.TemplatePath);

        LogCompletion($"{options.OutputPath}/dapr/{daprComponentResource.Name}.yaml");

        return Task.FromResult(true);
    }
}
