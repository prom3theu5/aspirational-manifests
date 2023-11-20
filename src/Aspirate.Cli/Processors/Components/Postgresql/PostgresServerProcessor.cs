namespace Aspirate.Cli.Processors.Components.Postgresql;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public partial class PostgresServerProcessor(IFileSystem fileSystem, ILogger<PostgresServerProcessor> logger) : BaseProcessor<PostgresServerTemplateData>(fileSystem, logger)
{
    private readonly IReadOnlyCollection<string> _manifests =
    [
        "postgres.yml",
    ];

    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.PostgresServer;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<PostgresServer>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        LogHandlerExecution(logger, nameof(PostgresServerProcessor), resourceOutputPath);

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new PostgresServerTemplateData(_manifests);

        CreateCustomManifest(resourceOutputPath, "postgres.yml", TemplateLiterals.PostgresServerType, data);
        CreateComponentKustomizeManifest(resourceOutputPath, data);

        return Task.FromResult(true);
    }
}
