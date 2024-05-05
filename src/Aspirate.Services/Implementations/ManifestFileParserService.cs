using Aspirate.Shared.Interfaces.Processors;

namespace Aspirate.Services.Implementations;

/// <inheritdoc />
/// <summary>
/// Initialises a new instance of <see cref="ManifestFileParserService"/>.
/// </summary>
/// <param name="fileSystem">The file system accessor.</param>
/// <param name="console">The ansi-console instance used for console interaction.</param>
/// <param name="serviceProvider">The service provider to resolve handlers from.</param>
public class ManifestFileParserService(
    IFileSystem fileSystem,
    IAnsiConsole console,
    IServiceProvider serviceProvider) : IManifestFileParserService
{
    /// <inheritdoc />
    public Dictionary<string, Resource> LoadAndParseAspireManifest(string manifestFile)
    {
        var resources = new Dictionary<string, Resource>();

        if (!fileSystem.File.Exists(manifestFile))
        {
            throw new InvalidOperationException($"The manifest file could not be loaded from: '{manifestFile}'");
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
                console.MarkupLine($"[yellow]Resource {resourceName} does not have a type. Skipping as UnsupportedResource.[/]");
                resources.Add(resourceName, new UnsupportedResource());
                continue;
            }

            var rawBytes = Encoding.UTF8.GetBytes(resourceElement.GetRawText());
            var reader = new Utf8JsonReader(rawBytes);

            var resource = serviceProvider.GetKeyedService<IResourceProcessor>(type) is { } handler
                ? handler.Deserialize(ref reader)
                : new UnsupportedResource();

            if (resource != null)
            {
                resource.Name = resourceName;
                resources.Add(resourceName, resource);
            }
        }

        return resources;
    }
}
