namespace Aspirate.Processors.Resources.Dapr;

public class DaprProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    IManifestWriter manifestWriter)
    : BaseResourceProcessor(fileSystem, console, manifestWriter)
{
    public override string ResourceType => AspireComponentLiterals.DaprSystem;

    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<DaprResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy, string? templatePath = null,
        bool? disableSecrets = false,
        bool? withPrivateRegistry = false,
        bool? withDashboard = false) =>
        // Do nothing for dapr, they are there for annotations on services.
        Task.FromResult(true);
}
