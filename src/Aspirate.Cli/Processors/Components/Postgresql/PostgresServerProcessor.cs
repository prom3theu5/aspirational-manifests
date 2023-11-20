namespace Aspirate.Cli.Processors.Components.Postgresql;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public class PostgresServerProcessor(IFileSystem fileSystem, ILogger<PostgresServerProcessor> logger) : BaseProcessor<PostgresServerTemplateData>(fileSystem, logger)
{

    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.PostgresServer;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<PostgresServer>(ref reader);
}
