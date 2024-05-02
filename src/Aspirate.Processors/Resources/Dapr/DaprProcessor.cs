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

    public override Task<bool> CreateManifests(CreateManifestsOptions options) =>
        // Do nothing for dapr, they are there for annotations on services.
        Task.FromResult(true);
}
