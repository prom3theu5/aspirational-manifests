namespace Aspirate.ManifestHandlers;

/// <summary>
/// Base class for all manifest handlers.
/// </summary>
public abstract class BaseHandler<TTemplateData> : IHandler where TTemplateData : BaseTemplateData
{
    protected readonly IFileSystem _fileSystem;

    protected static readonly Dictionary<string, string> _templateFileMapping = new()
    {
        [TemplateLiterals.DeploymentType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, "deployment.hbs"),
        [TemplateLiterals.ServiceType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, "service.hbs"),
        [TemplateLiterals.ComponentKustomizeType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, "kustomization.hbs"),
        // Add more as needed
    };

    /// <summary>
    /// Initialises a new instance of <see cref="BaseHandler{TTemplateData}"/>.
    /// </summary>
    /// <param name="fileSystem">The file system accessor.</param>
    protected BaseHandler(IFileSystem fileSystem) =>
        _fileSystem = fileSystem;

    /// <inheritdoc />
    public abstract string ResourceType { get; }

    /// <inheritdoc />
    public abstract Resource? Deserialize(ref Utf8JsonReader reader);

    /// <inheritdoc />
    public virtual bool CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        AnsiConsole.MarkupLine(
            $"[yellow]Handler {GetType().Name} has not been configured. CreateManifest must be overridden.[/]");

        return false;
    }

    /// <inheritdoc />
    public virtual void CreateFinalManifest(Dictionary<string, Resource> resources, string outputPath) =>
        AnsiConsole.MarkupLine(
            $"[yellow]Handler {GetType().Name} has not been configured. CreateFinalManifest must be overridden in the FinalHandler.[/]");

    protected void EnsureOutputDirectoryExistsAndIsClean(string outputPath)
    {
        if (_fileSystem.Directory.Exists(outputPath))
        {
            _fileSystem.Directory.Delete(outputPath, true);
        }

        _fileSystem.Directory.CreateDirectory(outputPath);
    }

    protected void CreateDeployment(string outputPath, TTemplateData data)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.DeploymentType, out var templateFile);
        var deploymentOutputPath = Path.Combine(outputPath, "deployment.yaml");

        CreateFile(templateFile, deploymentOutputPath, data);
    }

    protected void CreateService(string outputPath, TTemplateData data)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ServiceType, out var templateFile);
        var serviceOutputPath = Path.Combine(outputPath, "service.yaml");

        CreateFile(templateFile, serviceOutputPath, data);
    }

    protected void CreateComponentKustomizeManifest(string outputPath, TTemplateData data)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ComponentKustomizeType, out var templateFile);
        var kustomizeOutputPath = Path.Combine(outputPath, "kustomization.yaml");

        CreateFile(templateFile, kustomizeOutputPath, data);
    }

    private void CreateFile(string inputFile, string outputPath, TTemplateData data)
    {
        var template = _fileSystem.File.ReadAllText(inputFile);
        var handlebarTemplate = Handlebars.Compile(template);
        var output = handlebarTemplate(data);

        File.WriteAllText(outputPath, output);
    }
}
