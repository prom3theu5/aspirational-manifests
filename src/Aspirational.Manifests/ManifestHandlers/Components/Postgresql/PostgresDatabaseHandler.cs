namespace Aspirational.Manifests.ManifestHandlers.Components.Postgresql;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public class PostgresDatabaseHandler : BaseHandler<PostgresDatabaseTemplateData>
{
    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.PostgresDatabase;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<PostgresDatabase>(ref reader);
}
