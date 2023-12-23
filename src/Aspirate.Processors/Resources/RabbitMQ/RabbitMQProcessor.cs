namespace Aspirate.Processors.Resources.RabbitMQ;

/// <summary>
/// Handles producing the RabbitMq component as Kustomize manifest.
/// </summary>
public class RabbitMqProcessor(IFileSystem fileSystem, IAnsiConsole console,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    private readonly IReadOnlyCollection<string> _manifests =
    [
        $"{TemplateLiterals.RabbitMqType}.yml",
    ];

    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.RabbitMq;

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<RabbitMqResource>(ref reader);

    public override Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null, bool? disableSecrets = false)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        _manifestWriter.EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var data = new KubernetesDeploymentTemplateData()
            .SetName("rabbitmq")
            .SetManifests(_manifests)
            .Validate();

        _manifestWriter.CreateCustomManifest(resourceOutputPath, $"{TemplateLiterals.RabbitMqType}.yml", TemplateLiterals.RabbitMqType, data, templatePath);
        _manifestWriter.CreateComponentKustomizeManifest(resourceOutputPath, data, templatePath);

        LogCompletion(resourceOutputPath);

        return Task.FromResult(true);
    }

    public override ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource)
    {
        var response = new ComposeService();

        var servicePort = new Port
        {
            Target = 5672,
            Published = 5672,
        };

        var managementPort = new Port
        {
            Target = 15672,
            Published = 15672,
        };

        var environment = new Dictionary<string, string?>
        {
            ["RABBITMQ_DEFAULT_USER"] = "guest",
            ["RABBITMQ_DEFAULT_PASS"] = "guest",
        };

        response.Service = Builder.MakeService("rabbitmq-service")
            .WithImage("rabbitmq:3.8-management")
            .WithEnvironment(environment)
            .WithContainerName("rabbitmq-service")
            .WithPortMappings(servicePort, managementPort)
            .WithRestartPolicy(RestartMode.UnlessStopped)
            .Build();

        return response;
    }
}
