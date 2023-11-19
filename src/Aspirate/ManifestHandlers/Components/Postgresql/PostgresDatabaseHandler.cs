
namespace Aspirate.ManifestHandlers.Components.Postgresql;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public class PostgresDatabaseHandler(IFileSystem fileSystem) : BaseHandler<PostgresDatabaseTemplateData>(fileSystem)
{

    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.PostgresDatabase;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<PostgresDatabase>(ref reader);
}
