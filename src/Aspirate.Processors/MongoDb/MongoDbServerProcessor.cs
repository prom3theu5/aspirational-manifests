namespace Aspirate.Processors.MongoDb;

public sealed class MongoDbServerProcessor(IFileSystem fileSystem, IAnsiConsole console) : BaseProcessor<MongoDbServerTemplateData>(fileSystem, console)
{
    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.MongoDbServerType}.yml",
    ];

    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.MongoDbServer;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<MongoDbServer>(ref reader);

    public override Task<bool> CreateManifests(
        KeyValuePair<string, Resource> resource,
        string outputPath,
        string imagePullPolicy,
        string? templatePath,
        bool? disableSecrets = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new MongoDbServerTemplateData(_manifests);

        CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.MongoDbServerType}.yml", TemplateLiterals.MongoDbServerType, data, templatePath);
        CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }
}
