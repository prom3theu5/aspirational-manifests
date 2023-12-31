namespace Aspirate.Processors.Resources.Postgresql;

/// <summary>
/// Handles producing the Postgres component as Kustomize manifest.
/// </summary>
public class PostgresServerProcessor(IFileSystem fileSystem, IAnsiConsole console,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.PostgresServerType}.yml",
    ];

    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.PostgresServer;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<PostgresServerResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false,
        bool? withPrivateRegistry = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new KubernetesDeploymentTemplateData()
            .SetName("postgresql")
            .SetIsService(false)
            .SetWithPrivateRegistry(withPrivateRegistry.GetValueOrDefault())
            .SetManifests(_manifests)
            .Validate();

        _manifestWriter.CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.PostgresServerType}.yml", TemplateLiterals.PostgresServerType, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource)
    {
        var response = new ComposeService();

        var servicePort = new Port
        {
            Target = 5432,
            Published = 5432,
        };

        var environment = new Dictionary<string, string?>
        {
            ["POSTGRES_DB"] = "postgres",
            ["POSTGRES_USER"] = "postgres",
            ["POSTGRES_PASSWORD"] = "postgres",
        };

        response.Service = Builder.MakeService("postgres-service")
            .WithImage("postgres:latest")
            .WithEnvironment(environment)
            .WithContainerName("postgres-service")
            .WithPortMappings(servicePort)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .Build();

        return response;
    }
}
