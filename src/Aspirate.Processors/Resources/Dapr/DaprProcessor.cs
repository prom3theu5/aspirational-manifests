namespace Aspirate.Processors.Resources.Dapr;

public class DaprProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    public override string ResourceType => AspireComponentLiterals.DaprSystem;

    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<DaprResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy, string? templatePath = null,
        bool? disableSecrets = false) =>
        // Do nothing for dapr, they are there for annotations on services.
        Task.FromResult(true);
}
