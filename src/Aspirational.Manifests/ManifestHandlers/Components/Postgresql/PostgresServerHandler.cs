namespace Aspirational.Manifests.ManifestHandlers.Components.Postgresql;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public class PostgresServerHandler : BaseHandler<PostgresServerTemplateData>
{
    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.PostgresServer;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<PostgresServer>(ref reader);
}
