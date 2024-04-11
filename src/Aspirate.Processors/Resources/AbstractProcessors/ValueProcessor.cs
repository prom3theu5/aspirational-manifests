namespace Aspirate.Processors.Resources.AbstractProcessors;

public class ValueProcessor(IFileSystem fileSystem, IAnsiConsole console,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.Value;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<ValueResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false,
        bool? withPrivateRegistry = false,
        bool? withDashboard = false) =>
        // Do nothing for Value Resources, they are there for configuration.
        Task.FromResult(true);
}
