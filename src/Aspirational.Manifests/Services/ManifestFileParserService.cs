namespace Aspirational.Manifests.Services;

/// <inheritdoc />
public class ManifestFileParserService : IManifestFileParserService
{
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Initialises a new instance of <see cref="ManifestFileParserService"/>. 
    /// </summary>
    /// <param name="fileSystem">The file system accessor.</param>
    public ManifestFileParserService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <summary>
    /// Initialises a new instance of <see cref="ManifestFileParserService"/>. 
    /// </summary>
    public ManifestFileParserService() : this(fileSystem: new FileSystem())
    {
    }

    /// <inheritdoc />
    public Dictionary<string, Resource> LoadAndParseAspireManifest(string manifestFile)
    {
        var resources = new Dictionary<string, Resource>();
        
        if (!_fileSystem.File.Exists(manifestFile))
        {
            throw new InvalidOperationException("The input file does not exist.");
        }

        var inputJson = _fileSystem.File.ReadAllText(manifestFile);

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
                
            var resource = HandlerMapping.ResourceTypeToHandlerMap.TryGetValue(type, out var handler)
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