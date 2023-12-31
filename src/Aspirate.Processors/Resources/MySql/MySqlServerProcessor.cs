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
        var resource = JsonSerializer.Deserialize<MySqlServerResource>(ref reader);
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
        bool? disableSecrets = false,
        bool? withPrivateRegistry = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new MySqlServerTemplateData()
            .SetRootPassword(resource.Value.Env["RootPassword"])
            .SetName("mysql")
            .SetIsService(false)
            .SetWithPrivateRegistry(withPrivateRegistry.GetValueOrDefault())
            .SetManifests(_manifests)
            .Validate();

        _manifestWriter.CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.MysqlServerType}.yml", TemplateLiterals.MysqlServerType, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource)
    {
        var response = new ComposeService();

        var servicePort = new Port
        {
            Target = 3306,
            Published = 3306,
        };

        var environment = new Dictionary<string, string?>
        {
            ["MYSQL_ROOT_PASSWORD"] = resource.Value.Env["RootPassword"],
        };

        response.Service = Builder.MakeService("mysql-service")
            .WithImage("mysql:latest")
            .WithEnvironment(environment)
            .WithContainerName("mysql-service")
            .WithPortMappings(servicePort)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .Build();

        return response;
    }
}
