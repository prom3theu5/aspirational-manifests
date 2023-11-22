namespace Aspirate.Cli.Processors.Components.Final;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class FinalProcessor(IFileSystem fileSystem) : BaseProcessor<FinalTemplateData>(fileSystem)
{

    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.Final;

    /// <inheritdoc />
    public override Resource Deserialize(ref Utf8JsonReader reader) =>
        throw new NotImplementedException();

    public override void CreateFinalManifest(Dictionary<string, Resource> resources, string outputPath)
    {
        var manifests = resources.Select(x => x.Key).ToList();

        var templateData = new FinalTemplateData(manifests);

        CreateComponentKustomizeManifest(outputPath, templateData);
    }
}
