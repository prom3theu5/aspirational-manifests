namespace Aspirate.Cli.Processors.Components.Postgresql;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public class PostgresDatabaseProcessor(IFileSystem fileSystem, ILogger<PostgresDatabaseProcessor> logger) : BaseProcessor<PostgresDatabaseTemplateData>(fileSystem, logger)
{

    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.PostgresDatabase;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<PostgresDatabase>(ref reader);
}
