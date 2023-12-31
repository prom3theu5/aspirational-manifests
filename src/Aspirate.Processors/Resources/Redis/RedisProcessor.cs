namespace Aspirate.Processors.Resources.Redis;

/// <summary>
/// Handles producing the Redis component as Kustomize manifest.
/// </summary>
public class RedisProcessor(IFileSystem fileSystem, IAnsiConsole console,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.RedisType}.yml",
    ];

    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.Redis;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<RedisResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false,
        bool? withPrivateRegistry = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new KubernetesDeploymentTemplateData()
            .SetName("redis")
            .SetIsService(false)
            .SetManifests(_manifests)
            .Validate();

        _manifestWriter.CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.RedisType}.yml", TemplateLiterals.RedisType, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource)
    {
        var response = new ComposeService();

        var servicePort = new Port
        {
            Target = 6379,
            Published = 6379,
        };

        var environment = new Dictionary<string, string?>
        {
            ["ALLOW_EMPTY_PASSWORD"] = "yes",
        };

        response.Service = Builder.MakeService("redis")
            .WithImage("bitnami/redis:latest")
            .WithEnvironment(environment)
            .WithContainerName("redis")
            .WithPortMappings(servicePort)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .Build();

        return response;
    }
}
