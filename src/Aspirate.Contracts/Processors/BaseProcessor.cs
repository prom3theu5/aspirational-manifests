namespace Aspirate.Contracts.Processors;

/// <summary>
/// Base class for all manifest handlers.
/// </summary>
public abstract class BaseProcessor<TTemplateData> : IProcessor where TTemplateData : BaseTemplateData
{
    protected readonly IFileSystem _fileSystem;
    protected readonly IAnsiConsole _console;
    protected readonly string _defaultTemplatePath = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder);

    protected readonly Dictionary<string, string> _templateFileMapping = new()
    {
        [TemplateLiterals.DeploymentType] = $"{TemplateLiterals.DeploymentType}.hbs",
        [TemplateLiterals.ServiceType] = $"{TemplateLiterals.ServiceType}.hbs",
        [TemplateLiterals.ComponentKustomizeType] = $"{TemplateLiterals.ComponentKustomizeType}.hbs",
        [TemplateLiterals.RedisType] = $"{TemplateLiterals.RedisType}.hbs",
        [TemplateLiterals.RabbitMqType] = $"{TemplateLiterals.RabbitMqType}.hbs",
        [TemplateLiterals.PostgresServerType] = $"{TemplateLiterals.PostgresServerType}.hbs",
    };

    /// <summary>
    /// Initialises a new instance of <see cref="BaseProcessor{TTemplateData}"/>.
    /// </summary>
    /// <param name="fileSystem">The file system accessor.</param>
    /// <param name="console">The ansi-console instance used for console interaction.</param>
    protected BaseProcessor(IFileSystem fileSystem, IAnsiConsole console)
    {
        _fileSystem = fileSystem;
        _console = console;
    }

    /// <inheritdoc />
    public abstract string ResourceType { get; }

    /// <inheritdoc />
    public abstract Resource? Deserialize(ref Utf8JsonReader reader);

    /// <inheritdoc />
    public virtual Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, AspirateSettings? aspirateSettings = null)
    {
        _console.LogCreateManifestNotOverridden(GetType().Name);

        return Task.FromResult(false);
    }

    protected void EnsureOutputDirectoryExistsAndIsClean(string outputPath)
    {
        if (_fileSystem.Directory.Exists(outputPath))
        {
            _fileSystem.Directory.Delete(outputPath, true);
        }

        _fileSystem.Directory.CreateDirectory(outputPath);
    }

    protected void CreateDeployment(string outputPath, TTemplateData data, AspirateSettings? aspirateSettings = null)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.DeploymentType, out var templateFile);
        var deploymentOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.DeploymentType}.yml");

        CreateFile(templateFile, deploymentOutputPath, data, aspirateSettings);
    }

    protected void CreateService(string outputPath, TTemplateData data, AspirateSettings? aspirateSettings = null)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ServiceType, out var templateFile);
        var serviceOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.ServiceType}.yml");

        CreateFile(templateFile, serviceOutputPath, data, aspirateSettings);
    }

    protected void CreateComponentKustomizeManifest(
        string outputPath,
        TTemplateData data,
        AspirateSettings? aspirateSettings = null)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ComponentKustomizeType, out var templateFile);
        var kustomizeOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.ComponentKustomizeType}.yml");

        CreateFile(templateFile, kustomizeOutputPath, data, aspirateSettings);
    }

    protected void CreateCustomManifest(
        string outputPath,
        string fileName,
        string templateType,
        TTemplateData data,
        AspirateSettings? aspirateSettings = null)
    {
        _templateFileMapping.TryGetValue(templateType, out var templateFile);
        var deploymentOutputPath = Path.Combine(outputPath, fileName);

        CreateFile(templateFile, deploymentOutputPath, data, aspirateSettings);
    }

    private void CreateFile(string inputFile, string outputPath, TTemplateData data, AspirateSettings? aspirateSettings = null)
    {
        var templateFile = GetTemplateFilePath(inputFile, aspirateSettings);

        var template = _fileSystem.File.ReadAllText(templateFile);
        var handlebarTemplate = Handlebars.Compile(template);
        var output = handlebarTemplate(data);

        _fileSystem.File.WriteAllText(outputPath, output);
    }

    private string GetTemplateFilePath(string templateFile, AspirateSettings? aspirateSettings = null) =>
        Path.Combine(aspirateSettings?.TemplatePath ?? _defaultTemplatePath, templateFile);

    protected void LogCompletion(string outputPath) =>
        _console.LogCompletion(outputPath);
}
