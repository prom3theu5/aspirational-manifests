namespace Aspirate.Contracts.Processors;

/// <summary>
/// Base class for all manifest handlers.
/// </summary>
public abstract partial class BaseProcessor<TTemplateData> : IProcessor where TTemplateData : BaseTemplateData
{
    protected readonly IFileSystem _fileSystem;

    protected ILogger<BaseProcessor<TTemplateData>> _logger { get; }

    protected static readonly Dictionary<string, string> _templateFileMapping = new()
    {
        [TemplateLiterals.DeploymentType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, "deployment.hbs"),
        [TemplateLiterals.ServiceType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, "service.hbs"),
        [TemplateLiterals.ComponentKustomizeType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, "kustomization.hbs"),
        // Add more as needed
    };

    /// <summary>
    /// Initialises a new instance of <see cref="BaseProcessor{TTemplateData}"/>.
    /// </summary>
    /// <param name="fileSystem">The file system accessor.</param>
    /// <param name="logger">The logging instance.</param>
    protected BaseProcessor(IFileSystem fileSystem, ILogger<BaseProcessor<TTemplateData>> logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
    }

    /// <inheritdoc />
    public abstract string ResourceType { get; }

    /// <inheritdoc />
    public abstract Resource? Deserialize(ref Utf8JsonReader reader);

    /// <inheritdoc />
    public virtual Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        LogMustOverrideCreateManifest(_logger, GetType().Name);

        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public virtual void CreateFinalManifest(Dictionary<string, Resource> resources, string outputPath) =>
        LogMustOverrideCreateFinalManifest(_logger, GetType().Name);

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

        _fileSystem.File.WriteAllText(outputPath, output);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Handler {Handler} has not been configured. CreateManifest must be overridden.")]
    static partial void LogMustOverrideCreateManifest(ILogger logger, string handler);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Handler {Handler} has not been configured. CreateFinalManifest must be overridden in the FinalHandler.")]
    static partial void LogMustOverrideCreateFinalManifest(ILogger logger, string handler);
}
