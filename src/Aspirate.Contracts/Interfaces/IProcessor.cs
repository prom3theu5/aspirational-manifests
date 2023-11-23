namespace Aspirate.Contracts.Interfaces;

public interface IProcessor
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
    Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath);
}
