namespace Aspirate.Processors.Postgresql;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public class PostgresDatabaseProcessor(IFileSystem fileSystem, IAnsiConsole console) : BaseProcessor<PostgresDatabaseTemplateData>(fileSystem, console)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.PostgresDatabase;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<PostgresDatabase>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false) =>
        // Do nothing for databases, they are there for configuration.
        Task.FromResult(true);
}
