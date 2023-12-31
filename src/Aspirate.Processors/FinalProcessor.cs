namespace Aspirate.Processors;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public sealed class FinalProcessor(IFileSystem fileSystem, IAnsiConsole console,
    IManifestWriter manifestWriter,
    IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    : BaseResourceProcessor(fileSystem, console, manifestWriter, substitutionStrategies)
{
    /// <inheritdoc />
    public override string ResourceType => AspireLiterals.Final;

    /// <inheritdoc />
    public override Resource Deserialize(ref Utf8JsonReader reader) =>
        throw new NotImplementedException();

    public void CreateFinalManifest(Dictionary<string, Resource> resources,
        string outputPath,
        string? templatePath = null,
        string? @namespace = null,
        bool? withPrivateRegistry = false,
        string? registryUsername = null,
        string? registryPassword = null,
        string? registryEmail = null)
    {
        var manifests = resources.Select(x => x.Key).ToList();

        var templateDataBuilder = new KubernetesDeploymentTemplateData()
            .SetNamespace(@namespace)
            .SetIsService(false);

        HandleNamespace(outputPath, templatePath, @namespace, templateDataBuilder, manifests);
        HandlePrivateRegistry(outputPath, withPrivateRegistry, registryUsername, registryPassword, registryEmail, manifests);

        HandleDapr(outputPath, manifests);

        _console.MarkupLine($"\r\n[bold]Generating final manifest with name [blue]'kustomization.yml'[/][/]");

        var templateData = templateDataBuilder.SetManifests(manifests);

        _manifestWriter.CreateComponentKustomizeManifest(outputPath, templateData, templatePath);

        _console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputPath}/kustomization.yml[/]");
    }

    private void HandlePrivateRegistry(string outputPath, bool? withPrivateRegistry, string? registryUsername, string? registryPassword, string? registryEmail, List<string> manifests)
    {
        if (!withPrivateRegistry.GetValueOrDefault())
        {
            return;
        }

        _console.MarkupLine($"\r\n[bold]Generating private registry secret manifest.[/]");
        _manifestWriter.CreateImagePullSecret(registryUsername, registryPassword, registryEmail, TemplateLiterals.ImagePullSecretType, outputPath);
        manifests.Add($"{TemplateLiterals.ImagePullSecretType}.yml");
        _console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputPath}/{TemplateLiterals.ImagePullSecretType}.yml[/]");
    }

    private void HandleNamespace(string outputPath, string? templatePath, string @namespace, KubernetesDeploymentTemplateData templateDataBuilder, List<string> manifests)
    {
        if (string.IsNullOrEmpty(@namespace))
        {
            return;
        }

        _console.MarkupLine($"\r\n[bold]Generating namespace manifest with name [blue]'{@namespace}'[/][/]");
        _manifestWriter.CreateNamespace(outputPath, templateDataBuilder, templatePath);
        manifests.Add($"{TemplateLiterals.NamespaceType}.yml");
        _console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputPath}/namespace.yml[/]");
    }

    private void HandleDapr(string outputPath, List<string> manifests)
    {
        if (!_fileSystem.Directory.Exists(Path.Combine(outputPath, "dapr")))
        {
            return;
        }

        var daprFiles = _fileSystem.Directory.GetFiles(Path.Combine(outputPath, "dapr"), "*.yml", SearchOption.AllDirectories);
        manifests.AddRange(daprFiles.Select(daprFile => daprFile.Replace(outputPath, string.Empty).TrimStart(Path.DirectorySeparatorChar)));
    }
}
