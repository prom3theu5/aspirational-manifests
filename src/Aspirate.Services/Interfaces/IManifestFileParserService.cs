namespace Aspirate.Services.Interfaces;

/// <summary>
/// Represents a service for loading and parsing Aspire manifest files.
/// </summary>
public interface IManifestFileParserService
{
    /// <summary>
    /// Loads and parses an Aspire manifest file.
    /// </summary>
    /// <param name="manifestFile">Path to the manifest file</param>
    /// <returns>A collection of resources in a dictionary.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the file does not exist.</exception>
    Dictionary<string, Resource> LoadAndParseAspireManifest(string manifestFile);
}
