namespace Aspirate.Processors.SqlServer;

public sealed class SqlServerProcessor(IFileSystem fileSystem, IAnsiConsole console, IPasswordGenerator passwordGenerator) : BaseProcessor<SqlServerTemplateData>(fileSystem, console)
{
    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.SqlServerType}.yml",
    ];

    /// <inheritdoc />
    public override string ResourceType => AspireLiterals.SqlServer;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader)
    {
        var resource = JsonSerializer.Deserialize<Shared.Models.AspireManifests.Components.V1.SqlServer>(ref reader);
        resource.Env = new()
        {
            ["SaPassword"] = passwordGenerator.Generate().ToBase64(),
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

        var data = new SqlServerTemplateData(_manifests)
        {
            SaPassword = resource.Value.Env["SaPassword"],
        };

        CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.SqlServerType}.yml", TemplateLiterals.SqlServerType, data, templatePath);
        CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }
}
