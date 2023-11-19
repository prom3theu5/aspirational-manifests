namespace Aspirational.Manifests.ManifestHandlers;

/// <summary>
/// Base class for all manifest handlers.
/// </summary>
public abstract class BaseHandler
{
    protected readonly IFileSystem _fileSystem;

    /// <summary>
    /// Initialises a new instance of <see cref="BaseHandler"/>.
    /// </summary>
    /// <param name="fileSystem">The file system accessor.</param>
    protected BaseHandler(IFileSystem fileSystem) =>
        _fileSystem = fileSystem;

    /// <summary>
    /// Initialises a new instance of <see cref="BaseHandler"/>.
    /// </summary>
    protected BaseHandler() : this(fileSystem: new FileSystem())
    {
    }

    /// <summary>
    /// The resource type that this handler is for.
    /// </summary>
    public abstract string ResourceType { get; }

    /// <summary>
    /// Serializes the resource to JSON.
    /// </summary>
    /// <param name="reader">The reader instance.</param>
    /// <returns>A Resource instance.</returns>
    public abstract Resource? Deserialize(ref Utf8JsonReader reader);

    /// <summary>
    /// Produces the output manifest file.
    /// </summary>
    public virtual bool CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        AnsiConsole.MarkupLine(
            $"[yellow]Handler {GetType().Name} has not been configured. CreateManifest must be overridden.[/]");

        return false;
    }

    public virtual void CreateFinalManifest(Dictionary<string, Resource> resources, string outputPath)
    {
        AnsiConsole.MarkupLine($"[green]Creating final kustomize manifest for aspire manifest[/]");

        var manifests = resources.Select(x => x.Key).ToList();

        var data = new
        {
            Manifests = manifests,
            IsService = false,
        };

        CreateComponentKustomizeManifest(outputPath, data);
    }

    protected void EnsureOutputDirectoryExistsAndIsClean(string outputPath)
    {
        if (_fileSystem.Directory.Exists(outputPath))
        {
            _fileSystem.Directory.Delete(outputPath, true);
        }

        _fileSystem.Directory.CreateDirectory(outputPath);
    }

    protected void CreateDeployment(string outputPath, object data)
    {
        HandlerMapping.TemplateFileMapping.TryGetValue(TemplateLiterals.DeploymentType, out var templateFile);
        var deploymentOutputPath = Path.Combine(outputPath, "deployment.yaml");

        CreateFile(templateFile, deploymentOutputPath, data);
    }

    protected void CreateService(string outputPath, object data)
    {
        HandlerMapping.TemplateFileMapping.TryGetValue(TemplateLiterals.ServiceType, out var templateFile);
        var serviceOutputPath = Path.Combine(outputPath, "service.yaml");

        CreateFile(templateFile, serviceOutputPath, data);
    }

    protected void CreateComponentKustomizeManifest(string outputPath, object data)
    {
        HandlerMapping.TemplateFileMapping.TryGetValue(TemplateLiterals.ComponentKustomizeType, out var templateFile);
        var kustomizeOutputPath = Path.Combine(outputPath, "kustomization.yaml");

        CreateFile(templateFile, kustomizeOutputPath, data);
    }

    private void CreateFile(string inputFile, string outputPath, object data)
    {
        var template = _fileSystem.File.ReadAllText(inputFile);
        var handlebarTemplate = Handlebars.Compile(template);
        var output = handlebarTemplate(data);

        File.WriteAllText(outputPath, output);
    }
}
