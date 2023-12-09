using Aspirate.Shared.Models.AspireManifests.Components.V1;

namespace Aspirate.Processors.SqlServer;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public class SqlServerDatabaseProcessor(IFileSystem fileSystem, IAnsiConsole console) : BaseProcessor<SqlServerDatabaseTemplateData>(fileSystem, console)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.SqlServerDatabase;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<SqlServerDatabase>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false) =>
        // Do nothing for databases, they are there for configuration.
        Task.FromResult(true);
}
