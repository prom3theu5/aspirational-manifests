namespace Aspirate.Processors.Resources.Azure;

public class AzureStorageProcessor(IFileSystem fileSystem, IAnsiConsole console,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.AzureStorage;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<AzureStorageResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false,
        bool? withPrivateRegistry = false,
        bool? withDashboard = false) =>
        // Do nothing for Azure Resources, they are there for configuration.
        Task.FromResult(true);
}
