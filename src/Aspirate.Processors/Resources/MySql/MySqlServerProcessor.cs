namespace Aspirate.Processors.Resources.MySql;

public sealed class MySqlServerProcessor(IFileSystem fileSystem, IAnsiConsole console, IPasswordGenerator passwordGenerator,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.MysqlServerType}.yml",
    ];

    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.MySqlServer;

    /// <inheritdoc />
    public override Resource Deserialize(ref Utf8JsonReader reader)
    {
        var resource = JsonSerializer.Deserialize<MySqlServer>(ref reader);
        resource.Env = new()
        {
            ["RootPassword"] = passwordGenerator.Generate().ToBase64(),
        };

        return resource;
    }

    public override Task<bool> CreateManifests(
        KeyValuePair<string, Resource> resource,
        string outputPath,
        string imagePullPolicy,
        string? templatePath,
        bool? disableSecrets = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new MySqlServerTemplateData()
            .SetRootPassword(resource.Value.Env["RootPassword"])
            .SetName("mysql")
            .SetManifests(_manifests)
            .Validate();

        _manifestWriter.CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.MysqlServerType}.yml", TemplateLiterals.MysqlServerType, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }
}
