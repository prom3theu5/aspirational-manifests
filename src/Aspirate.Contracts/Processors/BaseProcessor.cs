namespace Aspirate.Contracts.Processors;

/// <summary>
/// Base class for all manifest handlers.
/// </summary>
public abstract class BaseProcessor<TTemplateData> : IProcessor where TTemplateData : BaseTemplateData
{
    protected readonly IFileSystem _fileSystem;

    protected static readonly Dictionary<string, string> _templateFileMapping = new()
    {
        [TemplateLiterals.DeploymentType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, $"{TemplateLiterals.DeploymentType}.hbs"),
        [TemplateLiterals.ServiceType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, $"{TemplateLiterals.ServiceType}.hbs"),
        [TemplateLiterals.ComponentKustomizeType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, $"{TemplateLiterals.ComponentKustomizeType}.hbs"),
        [TemplateLiterals.RedisType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, $"{TemplateLiterals.RedisType}.hbs"),
        [TemplateLiterals.RabbitMqType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, $"{TemplateLiterals.RabbitMqType}.hbs"),
        [TemplateLiterals.PostgresServerType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, $"{TemplateLiterals.PostgresServerType}.hbs"),
    };

    /// <summary>
    /// Initialises a new instance of <see cref="BaseProcessor{TTemplateData}"/>.
    /// </summary>
    /// <param name="fileSystem">The file system accessor.</param>
    protected BaseProcessor(IFileSystem fileSystem) => _fileSystem = fileSystem;

    /// <inheritdoc />
    public abstract string ResourceType { get; }

    /// <inheritdoc />
    public abstract Resource? Deserialize(ref Utf8JsonReader reader);

    /// <inheritdoc />
    public virtual Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        AnsiConsole.MarkupLine($"\t[bold yellow]Handler {GetType().Name} has not been configured. CreateManifest must be overridden.[/]");

        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public virtual void CreateFinalManifest(Dictionary<string, Resource> resources, string outputPath) =>
        AnsiConsole.MarkupLine($"\t[bold yellow]Handler {GetType().Name} has not been configured. CreateFinalManifest must be overridden in the FinalHandler.[/]");

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
        var deploymentOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.DeploymentType}.yml");

        CreateFile(templateFile, deploymentOutputPath, data);
    }

    protected void CreateService(string outputPath, TTemplateData data)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ServiceType, out var templateFile);
        var serviceOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.ServiceType}.yml");

        CreateFile(templateFile, serviceOutputPath, data);
    }

    protected void CreateComponentKustomizeManifest(string outputPath, TTemplateData data)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ComponentKustomizeType, out var templateFile);
        var kustomizeOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.ComponentKustomizeType}.yml");

        CreateFile(templateFile, kustomizeOutputPath, data);
    }

    protected void CreateCustomManifest(string outputPath, string fileName, string templateType, TTemplateData data)
    {
        _templateFileMapping.TryGetValue(templateType, out var templateFile);
        var deploymentOutputPath = Path.Combine(outputPath, fileName);

        CreateFile(templateFile, deploymentOutputPath, data);
    }

    private void CreateFile(string inputFile, string outputPath, TTemplateData data)
    {
        var template = _fileSystem.File.ReadAllText(inputFile);
        var handlebarTemplate = Handlebars.Compile(template);
        var output = handlebarTemplate(data);

        _fileSystem.File.WriteAllText(outputPath, output);
    }

    protected static void LogCompletion(string outputPath) =>
        AnsiConsole.MarkupLine($"\t[green](✔) Done: [/] Generating [blue]{outputPath}[/]");
}
