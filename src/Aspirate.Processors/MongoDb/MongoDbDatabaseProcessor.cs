namespace Aspirate.Processors.MongoDb;

public class MongoDbDatabaseProcessor(IFileSystem fileSystem, IAnsiConsole console) : BaseProcessor<MongoDbDatabaseTemplateData>(fileSystem, console)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.MongoDbDatabase;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<MongoDbDatabase>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false) =>
        // Do nothing for databases, they are there for configuration.
        Task.FromResult(true);
}
