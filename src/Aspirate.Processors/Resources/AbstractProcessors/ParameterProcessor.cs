namespace Aspirate.Processors.Resources.AbstractProcessors;

public class ParameterProcessor(IFileSystem fileSystem, IAnsiConsole console,
    IManifestWriter manifestWriter)
    : BaseResourceProcessor(fileSystem, console, manifestWriter)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.Parameter;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<ParameterResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false,
        bool? withPrivateRegistry = false,
        bool? withDashboard = false) =>
        // Do nothing for Parameter Resources, they are there for configuration.
        Task.FromResult(true);
}
