
namespace Aspirate.ManifestHandlers.Components.Postgresql;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public class PostgresServerHandler(IFileSystem fileSystem) : BaseHandler<PostgresServerTemplateData>(fileSystem)
{

    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.PostgresServer;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<PostgresServer>(ref reader);
}
