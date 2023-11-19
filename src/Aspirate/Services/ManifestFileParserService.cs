namespace Aspirate.Services;

/// <inheritdoc />
/// <summary>
/// Initialises a new instance of <see cref="ManifestFileParserService"/>. 
/// </summary>
/// <param name="fileSystem">The file system accessor.</param>
/// <param name="serviceProvider">The service provider to resolve handlers from.</param>
public class ManifestFileParserService(IFileSystem fileSystem, IServiceProvider serviceProvider) : IManifestFileParserService
{
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

            var resource = serviceProvider.GetKeyedService<IHandler>(type) is { } handler
                ? handler.Deserialize(ref reader)
                : new UnsupportedResource();

            if (resource != null)
            {
                resources.Add(resourceName, resource);
            }
        }

        return resources;
    }
}
