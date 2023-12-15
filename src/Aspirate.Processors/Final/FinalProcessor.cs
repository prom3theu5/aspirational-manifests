namespace Aspirate.Processors.Final;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public sealed class FinalProcessor(IFileSystem fileSystem, IAnsiConsole console) : BaseProcessor<FinalTemplateData>(fileSystem, console)
{

    /// <inheritdoc />
    public override string ResourceType => AspireLiterals.Final;

    /// <inheritdoc />
    public override Resource Deserialize(ref Utf8JsonReader reader) =>
        throw new NotImplementedException();

    public void CreateFinalManifest(Dictionary<string, Resource> resources, string outputPath, string? templatePath = null, string? @namespace = null)
    {
        var manifests = resources.Select(x => x.Key).ToList();

        var templateData = new FinalTemplateData(manifests, @namespace);

        if (!string.IsNullOrEmpty(@namespace))
        {
            _console.MarkupLine($"\r\n[bold]Generating namespace manifest with name [blue]'{@namespace}'[/][/]");
            CreateNamespace(outputPath, templateData, templatePath);
            manifests.Add($"{TemplateLiterals.NamespaceType}.yml");
            _console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputPath}/namespace.yml[/]");
        }

        CreateComponentKustomizeManifest(outputPath, templateData, templatePath);

        _console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputPath}/kustomization.yml[/]");
    }
}
