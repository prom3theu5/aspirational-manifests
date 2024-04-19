namespace Aspirate.Shared.Processors;

public interface IResourceProcessor
{
    /// <summary>
    /// The resource type that this handler is for.
    /// </summary>
    string ResourceType { get; }

    /// <summary>
    /// Serializes the resource to JSON.
    /// </summary>
    /// <param name="reader">The reader instance.</param>
    /// <returns>A Resource instance.</returns>
    Resource? Deserialize(ref Utf8JsonReader reader);

    /// <summary>
    /// Produces the output manifest file.
    /// </summary>
    Task<bool> CreateManifests(
        KeyValuePair<string, Resource> resource,
        string outputPath,
        string imagePullPolicy,
        string? aspirateSettings = null,
        bool? disableSecrets = false,
        bool? withPrivateRegistry = false,
        bool? withDashboard = false);

    /// <summary>
    /// Creates a compose entry using the specified resource.
    /// </summary>
    /// <param name="resource">The key-value pair containing the compose entry's identifier and resource.</param>
    /// <param name="withDashboard">Should include the aspire dashboard.</param>
    /// <returns>The created compose entry as a <see cref="Service"/> object, or null if the operation fails.</returns>
    ComposeService CreateComposeEntry(KeyValuePair<string, Resource> resource, bool? withDashboard = false);
}
