namespace Aspirate.Processors;

/// <summary>
/// Base class for all manifest handlers.
/// </summary>
public abstract class BaseResourceProcessor : IResourceProcessor
{
    /// <summary>
    /// Represents the instance of a file system.
    /// </summary>
    protected readonly IFileSystem _fileSystem;

    /// <summary>
    /// Represents a protected and read-only instance of the <see cref="IAnsiConsole"/> interface.
    /// </summary>
    protected readonly IAnsiConsole _console;

    /// <summary>
    /// Represents an instance of a manifest writer.
    /// </summary>
    /// <remarks>
    /// The manifest writer is responsible for writing manifest files.
    /// </remarks>
    protected readonly IManifestWriter _manifestWriter;

    /// <summary>
    /// Represents the base processor class for handling template data.
    /// </summary>
    protected BaseResourceProcessor(
        IFileSystem fileSystem,
        IAnsiConsole console,
        IManifestWriter manifestWriter)
    {
        _fileSystem = fileSystem;
        _console = console;
        _manifestWriter = manifestWriter;
    }

    /// <inheritdoc />
    public abstract string ResourceType { get; }

    /// <summary>
    /// Deserializes JSON data from the provided <see cref="Utf8JsonReader"/> into an <see cref="Shared.Models.AspireManifests.Resource"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing the JSON data.</param>
    /// <returns>The deserialized <see cref="Shared.Models.AspireManifests.Resource"/> object, or null if the deserialization fails.</returns>
    public abstract Resource? Deserialize(ref Utf8JsonReader reader);

    /// <summary>
    /// Filters the environmental variables of a given resource and returns a dictionary of the filtered variables.
    /// </summary>
    /// <param name="resource">The resource whose environmental variables need to be filtered.</param>
    /// <param name="disableSecrets">if secrets are disabled, do not filter</param>
    /// <param name="withDashboard">Should the dashboard be included.</param>
    /// <returns>A dictionary representing the filtered environmental variables.</returns>
    protected Dictionary<string, string?> GetFilteredEnvironmentalVariables(KeyValuePair<string, Resource> resource, bool? disableSecrets, bool? withDashboard)
    {
        var resourceWithEnv = resource.MapResourceToEnvVars(withDashboard);

        return disableSecrets == true ? resourceWithEnv : resourceWithEnv.Where(e => !ProtectorType.List.Any(p => e.Key.StartsWith(p))).ToDictionary(e => e.Key, e => e.Value);
    }

    /// <summary>
    /// Filters the environmental variables of a given resource and returns a dictionary of the secret variables.
    /// </summary>
    /// <param name="resource">The resource from which to retrieve the secret environmental variables.</param>
    /// <param name="disableSecrets">if secrets are disabled, do not filter</param>
    /// <param name="withDashboard">Should the dashboard be included.</param>
    /// <returns>A dictionary representing the secret environmental variables.</returns>
    protected Dictionary<string, string?> GetSecretEnvironmentalVariables(KeyValuePair<string, Resource> resource, bool? disableSecrets, bool? withDashboard)
    {
        var resourceWithEnv = resource.MapResourceToEnvVars(withDashboard);

        return resourceWithEnv.Where(e => ProtectorType.List.Any(p => e.Key.StartsWith(p))).ToDictionary(e => e.Key, e => e.Value);
    }

    /// <inheritdoc />
    public virtual Task<bool> CreateManifests(CreateManifestsOptions options)
    {
        LogCreateManifestNotOverridden(GetType().Name);

        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public virtual List<object> CreateKubernetesObjects(CreateKubernetesObjectsOptions options)
    {
        LogCreateKubernetesObjectsNotOverridden(GetType().Name);

        return [];
    }

    /// <inheritdoc />
    public virtual ComposeService CreateComposeEntry(CreateComposeEntryOptions options)
    {
        LogCreateComposeNotOverridden(GetType().Name);

        return null;
    }

    /// <summary>
    /// Logs the completion of a task with the given output path.
    /// </summary>
    /// <param name="outputPath">The path of the output file or directory.</param>
    protected void LogCompletion(string outputPath) =>
        LogCompletionMessage(_fileSystem.GetFullPath(outputPath));

    private void LogCreateManifestNotOverridden(string processor) =>
        _console.MarkupLine($"[bold yellow]Processor {processor} has not been configured. CreateManifest must be overridden.[/]");

    private void LogCreateComposeNotOverridden(string processor) =>
        _console.MarkupLine($"[bold yellow]Processor {processor} has not been configured. CreateComposeEntry must be overridden.[/]");

    private void LogCreateKubernetesObjectsNotOverridden(string processor) =>
        _console.MarkupLine($"[bold yellow]Processor {processor} has not been configured. CreateKubernetesObjects must be overridden.[/]");

    private void LogCompletionMessage(string outputPath) => _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputPath}[/]");
}
