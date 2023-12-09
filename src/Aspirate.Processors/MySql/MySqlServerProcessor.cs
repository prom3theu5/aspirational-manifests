namespace Aspirate.Processors.MySql;

public sealed class MySqlServerProcessor(IFileSystem fileSystem, IAnsiConsole console, IPasswordGenerator passwordGenerator) : BaseProcessor<MySqlServerTemplateData>(fileSystem, console)
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

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new MySqlServerTemplateData(_manifests)
        {
            RootPassword = resource.Value.Env["RootPassword"],
        };

        CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.MysqlServerType}.yml", TemplateLiterals.MysqlServerType, data, templatePath);
        CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }
}
