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
    /// The list of placeholder substitution strategies used for string formatting.
    /// </summary>
    protected readonly List<IPlaceholderSubstitutionStrategy> _substitutionStrategies;

    /// <summary>
    /// Represents the base processor class for handling template data.
    /// </summary>
    protected BaseResourceProcessor(
        IFileSystem fileSystem,
        IAnsiConsole console,
        IManifestWriter manifestWriter,
        IEnumerable<IPlaceholderSubstitutionStrategy>? substitutionStrategies)
    {
        _fileSystem = fileSystem;
        _console = console;
        _manifestWriter = manifestWriter;
        _substitutionStrategies = substitutionStrategies?.ToList() ?? [];
    }

    /// <inheritdoc />
    public abstract string ResourceType { get; }

    /// <summary>
    /// Deserializes JSON data from the provided <see cref="Utf8JsonReader"/> into a <see cref="Resource"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing the JSON data.</param>
    /// <returns>The deserialized <see cref="Resource"/> object, or null if the deserialization fails.</returns>
    public abstract Resource? Deserialize(ref Utf8JsonReader reader);

    /// <summary>
    /// Filters the environmental variables of a given resource and returns a dictionary of the filtered variables.
    /// </summary>
    /// <param name="resource">The resource whose environmental variables need to be filtered.</param>
    /// <param name="disableSecrets">if secrets are disabled, do not filter</param>
    /// <returns>A dictionary representing the filtered environmental variables.</returns>
    protected Dictionary<string, string> GetFilteredEnvironmentalVariables(Resource resource, bool? disableSecrets = false)
    {
        if (disableSecrets == true)
        {
            return resource.Env;
        }

        var envVars = resource.Env;

        return envVars == null ? [] : envVars.Where(e => !ProtectorType.List.Any(p => e.Key.StartsWith(p))).ToDictionary(e => e.Key, e => e.Value);
    }

    /// <summary>
    /// Filters the environmental variables of a given resource and returns a dictionary of the secret variables.
    /// </summary>
    /// <param name="resource">The resource from which to retrieve the secret environmental variables.</param>
    /// <param name="disableSecrets">if secrets are disabled, do not filter</param>
    /// <returns>A dictionary representing the secret environmental variables.</returns>
    protected Dictionary<string, string> GetSecretEnvironmentalVariables(Resource resource, bool? disableSecrets = false)
    {
        if (disableSecrets == true)
        {
            return [];
        }

        var envVars = resource.Env;

        return envVars == null ? [] : envVars.Where(e => ProtectorType.List.Any(p => e.Key.StartsWith(p))).ToDictionary(e => e.Key, e => e.Value);
    }

    /// <summary>
    /// Creates manifests for a resource.
    /// </summary>
    /// <param name="resource">The key-value pair representing the resource and its name.</param>
    /// <param name="outputPath">The path where the manifests will be created.</param>
    /// <param name="imagePullPolicy">The image pull policy for the resource.</param>
    /// <param name="templatePath">Optional. The path to the template used for creating the manifests.</param>
    /// <param name="disableSecrets">Passing this will disable all secret generation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating if the manifests were created successfully.</returns>
    public virtual Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null,
        bool? disableSecrets = false)
    {
        LogCreateManifestNotOverridden(GetType().Name);

        return Task.FromResult(false);
    }

    /// <summary>
    /// Logs the completion of a task with the given output path.
    /// </summary>
    /// <param name="outputPath">The path of the output file or directory.</param>
    protected void LogCompletion(string outputPath) =>
        LogCompletionMessage(_fileSystem.GetFullPath(outputPath));

    public virtual void ReplacePlaceholders(Resource resource, Dictionary<string, Resource> resources)
    {
        if (resource.Env is null)
        {
            return;
        }

        foreach (var entry in resource.Env)
        {
            if (!entry.Value.StartsWith('{') || !entry.Value.EndsWith('}'))
            {
                continue;
            }

            var strategy = _substitutionStrategies.FirstOrDefault(s => s.CanSubstitute(entry));

            strategy?.Substitute(entry, resources, resource);
        }
    }

    private void LogCreateManifestNotOverridden(string processor) =>
        _console.MarkupLine($"[bold yellow]Processor {processor} has not been configured. CreateManifest must be overridden.[/]");

    private void LogCompletionMessage(string outputPath) => _console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputPath}[/]");
}
