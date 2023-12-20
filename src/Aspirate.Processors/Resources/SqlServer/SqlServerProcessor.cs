namespace Aspirate.Processors.Resources.SqlServer;

public sealed class SqlServerProcessor(IFileSystem fileSystem, IAnsiConsole console, IPasswordGenerator passwordGenerator,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.SqlServerType}.yml",
    ];

    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.SqlServer;

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
        string? templatePath = null,
        bool? disableSecrets = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new SqlServerTemplateData()
            .SetSaPassword(resource.Value.Env["SaPassword"])
            .SetName("sqlserver")
            .SetManifests(_manifests)
            .Validate();

        _manifestWriter.CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.SqlServerType}.yml", TemplateLiterals.SqlServerType, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }
}
