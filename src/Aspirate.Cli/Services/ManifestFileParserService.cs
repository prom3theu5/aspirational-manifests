namespace Aspirate.Cli.Services;

/// <inheritdoc />
/// <summary>
/// Initialises a new instance of <see cref="ManifestFileParserService"/>.
/// </summary>
/// <param name="fileSystem">The file system accessor.</param>
/// <param name="serviceProvider">The service provider to resolve handlers from.</param>
public class ManifestFileParserService(IFileSystem fileSystem, IServiceProvider serviceProvider) : IManifestFileParserService
{
    private static readonly Dictionary<string, Func<string, string>> _aspireTypeHandlers = new()
    {
        [AspireLiterals.PostgresDatabase] = resourceName => $"host=postgres-service;database={resourceName};username=postgres;password=postgres;",
        [AspireLiterals.RabbitMq] = _ => "amqp://guest:guest@rabbitmq-service:5672",
        [AspireLiterals.Redis] = _ => "redis",
    };

    private static readonly Dictionary<string, Func<string, string>> _bindingHandlers = new()
    {
        ["bindings.http.url"] = serviceName => $"http://{serviceName}:8080",
        ["bindings.https.url"] = serviceName => $"https://{serviceName}:8443",
    };

    /// <inheritdoc />
    public Dictionary<string, Resource> LoadAndParseAspireManifest(string manifestFile)
    {
        var resources = new Dictionary<string, Resource>();

        if (!fileSystem.File.Exists(manifestFile))
        {
            throw new InvalidOperationException("The input file does not exist.");
        }

        var inputJson = fileSystem.File.ReadAllText(manifestFile);

        var jsonObject = JsonSerializer.Deserialize<JsonElement>(inputJson);

        if (!jsonObject.TryGetProperty("resources", out var resourcesElement) || resourcesElement.ValueKind != JsonValueKind.Object)
        {
            return resources;
        }

        foreach (var resourceProperty in resourcesElement.EnumerateObject())
        {
            var resourceName = resourceProperty.Name;
            var resourceElement = resourceProperty.Value;

            var type = resourceElement.TryGetProperty("type", out var typeElement) ? typeElement.GetString() : null;

            if (type == null)
            {
                AnsiConsole.MarkupLine($"[yellow]Resource {resourceName} does not have a type. Skipping as UnsupportedResource.[/]");
                resources.Add(resourceName, new UnsupportedResource());
                continue;
            }

            var rawBytes = Encoding.UTF8.GetBytes(resourceElement.GetRawText());
            var reader = new Utf8JsonReader(rawBytes);

            var resource = serviceProvider.GetKeyedService<IProcessor>(type) is { } handler
                ? handler.Deserialize(ref reader)
                : new UnsupportedResource();

            if (resource != null)
            {
                resources.Add(resourceName, resource);
            }
        }

        ReplacePlaceholdersInParsedResources(resources);

        return resources;
    }

    private static void ReplacePlaceholdersInParsedResources(Dictionary<string, Resource> resources)
    {
        foreach (var resource in resources.Values)
        {
            if (resource.Env == null)
            {
                continue;
            }

            foreach (var key in resource.Env.Keys)
            {
                var value = resource.Env[key];

                if (!value.StartsWith('{') || !value.EndsWith('}'))
                {
                    continue;
                }

                var parts = value.Trim('{', '}').Split('.');
                var resourceName = parts[0];
                var resourceType = resources[resourceName].Type;
                var propertyPath = string.Join('.', parts.Skip(1));

                if (_aspireTypeHandlers.TryGetValue(resourceType, out var typeHandler))
                {
                    resource.Env[key] = typeHandler(resourceName);
                    continue;
                }

                if (_bindingHandlers.TryGetValue(propertyPath, out var bindingHandler))
                {
                    resource.Env[key] = bindingHandler(resourceName);
                }
            }
        }
    }
}
