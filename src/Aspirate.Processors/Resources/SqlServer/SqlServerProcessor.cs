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
        var resource = JsonSerializer.Deserialize<SqlServerResource>(ref reader);
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
        bool? disableSecrets = false,
        bool? withPrivateRegistry = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new SqlServerTemplateData()
            .SetSaPassword(resource.Value.Env["SaPassword"])
            .SetName("sqlserver")
            .SetIsService(false)
            .SetManifests(_manifests)
            .Validate();

        _manifestWriter.CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.SqlServerType}.yml", TemplateLiterals.SqlServerType, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource)
    {
        var response = new ComposeService();

        var servicePort = new Port
        {
            Target = 1433,
            Published = 1433,
        };

        var environment = new Dictionary<string, string?>
        {
            ["ACCEPT_EULA"] = "Y",
            ["MSSQL_SA_PASSWORD"] = resource.Value.Env["SaPassword"],
        };

        response.Service = Builder.MakeService("sqlserver-service")
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment(environment)
            .WithContainerName("sqlserver-service")
            .WithPortMappings(servicePort)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .Build();

        return response;
    }
}
