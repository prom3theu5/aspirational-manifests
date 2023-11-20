
namespace Aspirate.Cli.Processors.Components.Final;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public partial class FinalProcessor(IFileSystem fileSystem, ILogger<FinalProcessor> logger) : BaseProcessor<FinalTemplateData>(fileSystem, logger)
{

    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.Final;

    /// <inheritdoc />
    public override Resource Deserialize(ref Utf8JsonReader reader) =>
        throw new NotImplementedException();

    public override void CreateFinalManifest(Dictionary<string, Resource> resources, string outputPath)
    {
        LogHandlerExecution(logger, outputPath);

        var manifests = resources.Select(x => x.Key).ToList();

        var templateData = new FinalTemplateData(manifests);

        CreateComponentKustomizeManifest(outputPath, templateData);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Creating final kustomize manifest for aspire manifest at output path: {OutputPath}")]
    static partial void LogHandlerExecution(ILogger logger, string outputPath);
}
