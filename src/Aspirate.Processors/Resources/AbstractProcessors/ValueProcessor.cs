namespace Aspirate.Processors.Resources.AbstractProcessors;

public class ValueProcessor(IFileSystem fileSystem, IAnsiConsole console,
    IManifestWriter manifestWriter)
    : BaseResourceProcessor(fileSystem, console, manifestWriter)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.Value;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<ValueResource>(ref reader);

    public override Task<bool> CreateManifests(CreateManifestsOptions options) =>
        // Do nothing for Value Resources, they are there for configuration.
        Task.FromResult(true);
}
